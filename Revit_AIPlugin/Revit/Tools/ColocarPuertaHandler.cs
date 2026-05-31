#nullable disable
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Revit_AIPlugin.Logging;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Coloca una puerta (host-based) sobre el muro más cercano a las coordenadas especificadas.
    /// Las coordenadas X,Y deben caer sobre los ejes perimetrales de la habitación.
    /// </summary>
    public class ColocarPuertaHandler : IExternalEventHandler
    {
        public string TipoPuerta { get; set; } = "Single";
        public double X { get; set; } = 0.0;
        public double Y { get; set; } = 0.0;
        public string Resultado { get; set; } = null;
        public TaskCompletionSource<string> TaskCompletionSource { get; set; }

        public void Execute(UIApplication app)
        {
            var stopwatch = Stopwatch.StartNew();
            Resultado = null;
            try
            {
                RevitAILogger.Info("Iniciando ColocarPuerta: TipoPuerta={TipoPuerta}, X={X}m, Y={Y}m", TipoPuerta, X, Y);

                Document doc = app.ActiveUIDocument.Document;
                Level nivelActual = ObtenerNivelActual(doc);

                double xInternal = UnitUtils.ConvertToInternalUnits(X, UnitTypeId.Meters);
                double yInternal = UnitUtils.ConvertToInternalUnits(Y, UnitTypeId.Meters);
                XYZ puntoPuerta = new XYZ(xInternal, yInternal, nivelActual.Elevation);

                RevitAILogger.Debug("Punto convertido a unidades internas: {Punto}", puntoPuerta);

                FamilySymbol simboloPuerta = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .FirstOrDefault(s =>
                        s.Name.IndexOf(TipoPuerta, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.Family.Name.IndexOf(TipoPuerta, StringComparison.OrdinalIgnoreCase) >= 0);

                if (simboloPuerta == null)
                {
                    RevitAILogger.Warn("No se encontró familia de puerta con tipo '{TipoPuerta}'. Usando fallback.", TipoPuerta);
                    simboloPuerta = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Doors)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .FirstOrDefault();
                }

                if (simboloPuerta == null)
                {
                    Resultado = "Error: No hay ninguna familia de puertas cargada en este proyecto.";
                    RevitAILogger.Error(null, "No hay ninguna familia de puertas cargada en el proyecto");
                    return;
                }

                RevitAILogger.Debug("Familia encontrada: {FamilyName}:{SymbolName}", simboloPuerta.Family.Name, simboloPuerta.Name);

                Wall muroHost = EncontrarMuroMasCercano(doc, puntoPuerta, nivelActual);

                if (muroHost == null)
                {
                    Resultado = "Error: No se encontró un muro host compatible para colocar la puerta.";
                    RevitAILogger.Error(null, "No se encontró muro anfitrión para colocar puerta en X={X}, Y={Y}", X, Y);
                    return;
                }

                RevitAILogger.Debug("Muro anfitrión encontrado: ID={WallId}", muroHost.Id.Value);

                using Transaction tx = new Transaction(doc, "Colocar Puerta IA");
                tx.Start();

                if (!simboloPuerta.IsActive)
                {
                    simboloPuerta.Activate();
                    doc.Regenerate();
                }

                doc.Create.NewFamilyInstance(puntoPuerta, simboloPuerta, muroHost, nivelActual, StructuralType.NonStructural);

                tx.Commit();

                stopwatch.Stop();
                Resultado = $"✅ Puerta colocada: '{simboloPuerta.Family.Name} : {simboloPuerta.Name}' en X={X}m, Y={Y}m.";
                RevitAILogger.Info("✅ Puerta colocada exitosamente: {Family}:{Symbol} en X={X}m, Y={Y}m (Duracion: {Ms}ms)",
                    simboloPuerta.Family.Name, simboloPuerta.Name, X, Y, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Resultado = $"Error Revit: {ex.GetType().Name} - {ex.Message}";
                RevitAILogger.Error(ex, "Error al colocar puerta (Duracion: {Ms}ms)", stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                stopwatch.Stop();
                TaskCompletionSource?.TrySetResult(Resultado ?? "Error: Sin respuesta del handler.");
                TaskCompletionSource = null;
            }
        }

        private Level ObtenerNivelActual(Document doc)
        {
            Level nivelActual = doc.ActiveView?.GenLevel;
            if (nivelActual != null)
            {
                return nivelActual;
            }

            return new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .OrderBy(l => l.Elevation)
                .FirstOrDefault() ?? throw new InvalidOperationException("No se encontró ningún nivel en el documento activo.");
        }

        /// <summary>
        /// Encuentra el muro más cercano al punto especificado, filtrando por nivel.
        /// Utiliza la LocationCurve del muro para calcular distancia al punto.
        /// </summary>
        private Wall EncontrarMuroMasCercano(Document doc, XYZ punto, Level nivel)
        {
            try
            {
                var muros = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .OfClass(typeof(Wall))
                    .Cast<Wall>()
                    .Where(w => w.LevelId == nivel.Id)
                    .ToList();

                RevitAILogger.Debug("Se encontraron {WallCount} muros en el nivel {LevelName}", muros.Count, nivel.Name);

                if (muros.Count == 0)
                {
                    muros = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Walls)
                        .OfClass(typeof(Wall))
                        .Cast<Wall>()
                        .ToList();

                    RevitAILogger.Warn("No había muros en el nivel actual. Se usará búsqueda global con {WallCount} muros.", muros.Count);
                }

                if (muros.Count == 0)
                {
                    return null;
                }

                Wall muroMasCercano = null;
                double distanciaMinima = double.MaxValue;

                foreach (var muro in muros)
                {
                    try
                    {
                        LocationCurve locationCurve = muro.Location as LocationCurve;
                        if (locationCurve?.Curve == null)
                        {
                            continue;
                        }

                        Curve curve = locationCurve.Curve;
                        IntersectionResult proyeccion = curve.Project(punto);

                        if (proyeccion != null)
                        {
                            double distancia = punto.DistanceTo(proyeccion.XYZPoint);

                            if (distancia < distanciaMinima)
                            {
                                distanciaMinima = distancia;
                                muroMasCercano = muro;
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (muroMasCercano != null)
                {
                    RevitAILogger.Debug("Muro más cercano encontrado: ID={WallId}, Distancia={Distance}m",
                        muroMasCercano.Id.Value,
                        UnitUtils.ConvertFromInternalUnits(distanciaMinima, UnitTypeId.Meters));
                }

                return muroMasCercano;
            }
            catch (Exception ex)
            {
                RevitAILogger.Error(ex, "Error en EncontrarMuroMasCercano");
                throw;
            }
        }

        public string GetName() => "ColocarPuertaHandler";
    }
}
