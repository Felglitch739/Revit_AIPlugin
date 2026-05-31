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
    /// Coloca una ventana (host-based) sobre el muro más cercano a las coordenadas especificadas.
    /// Las coordenadas X,Y deben caer sobre los ejes perimetrales de la habitación.
    /// </summary>
    public class ColocarVentanaHandler : IExternalEventHandler
    {
        public string TipoVentana { get; set; } = "Fixed";
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
                RevitAILogger.Info("Iniciando ColocarVentana: TipoVentana={TipoVentana}, X={X}m, Y={Y}m",
                    TipoVentana, X, Y);

                Document doc = app.ActiveUIDocument.Document;

                // Convertir coordenadas de metros a unidades internas de Revit (pies)
                double xInternal = UnitUtils.ConvertToInternalUnits(X, UnitTypeId.Meters);
                double yInternal = UnitUtils.ConvertToInternalUnits(Y, UnitTypeId.Meters);

                Level nivelActual = ObtenerNivelActual(doc);
                XYZ puntoVentana = new XYZ(xInternal, yInternal, nivelActual.Elevation);

                RevitAILogger.Debug("Punto convertido a unidades internas: {Punto}", puntoVentana);

                // Buscar familia de ventanas
                FamilySymbol simboloVentana = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Windows)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .FirstOrDefault(s =>
                        s.Name.IndexOf(TipoVentana, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.Family.Name.IndexOf(TipoVentana, StringComparison.OrdinalIgnoreCase) >= 0);

                // Fallback: usar la primera ventana disponible si no se especifica o no se encuentra
                if (simboloVentana == null)
                {
                    RevitAILogger.Warn("No se encontró familia de ventana con tipo '{TipoVentana}'. Usando fallback.", TipoVentana);

                    simboloVentana = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Windows)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .FirstOrDefault();
                }

                if (simboloVentana == null)
                {
                    RevitAILogger.Error(null, "No hay ninguna familia de ventanas cargada en el proyecto");
                    Resultado = "Error: No hay ninguna familia de ventanas cargada en este proyecto.";
                    return;
                }

                RevitAILogger.Debug("Familia encontrada: {FamilyName}:{SymbolName}",
                    simboloVentana.Family.Name, simboloVentana.Name);

                // Buscar el muro más cercano a las coordenadas especificadas
                Wall muroHost = EncontrarMuroMasCercano(doc, puntoVentana, nivelActual);

                if (muroHost == null)
                {
                    RevitAILogger.Error(null, "No se encontró muro anfitrión para colocar ventana en X={X}, Y={Y}", X, Y);
                    Resultado = "Error: No se encontró un muro host compatible para colocar la ventana.";
                    return;
                }

                RevitAILogger.Debug("Muro anfitrión encontrado: ID={WallId}", muroHost.Id.Value);

                using Transaction tx = new Transaction(doc, "Colocar Ventana IA");
                tx.Start();

                if (!simboloVentana.IsActive)
                {
                    simboloVentana.Activate();
                    doc.Regenerate();
                }

                // Crear instancia de familia con el muro como anfitrión
                doc.Create.NewFamilyInstance(puntoVentana, simboloVentana, muroHost, nivelActual, StructuralType.NonStructural);

                tx.Commit();

                stopwatch.Stop();
                RevitAILogger.Info("✅ Ventana colocada exitosamente: {Family}:{Symbol} en X={X}m, Y={Y}m (Duracion: {Ms}ms)",
                    simboloVentana.Family.Name, simboloVentana.Name, X, Y, stopwatch.ElapsedMilliseconds);

                Resultado = $"✅ Ventana colocada: '{simboloVentana.Family.Name} : {simboloVentana.Name}' en X={X}m, Y={Y}m.";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                RevitAILogger.Error(ex, "Error al colocar ventana (Duracion: {Ms}ms)", stopwatch.ElapsedMilliseconds);
                Resultado = $"Error Revit: {ex.GetType().Name} - {ex.Message}";
            }
            finally
            {
                stopwatch.Stop();
                TaskCompletionSource?.TrySetResult(Resultado ?? "Error: Sin respuesta del handler.");
                TaskCompletionSource = null;
            }
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

        public string GetName() => "ColocarVentanaHandler";
    }
}
