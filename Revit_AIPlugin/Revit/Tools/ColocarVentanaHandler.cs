#nullable disable
using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Handler para colocar ventanas en muros existentes.
    /// </summary>
    public class ColocarVentanaHandler : IExternalEventHandler, IHandlerConTCS
    {
        public string TipoVentana { get; set; } = "Fixed";
        public double X { get; set; } = 0.0;
        public double Y { get; set; } = 0.0;
        public string Resultado { get; set; } = null;
        public TaskCompletionSource<string> TaskCompletionSource { get; set; }

        public void Execute(UIApplication app)
        {
            Resultado = null;
            try
            {
                Document doc = app.ActiveUIDocument.Document;
                Level nivelActual = ObtenerNivelActual(doc);

                double xInternal = UnitUtils.ConvertToInternalUnits(X, UnitTypeId.Meters);
                double yInternal = UnitUtils.ConvertToInternalUnits(Y, UnitTypeId.Meters);
                XYZ puntoVentana = new XYZ(xInternal, yInternal, nivelActual.Elevation);

                FamilySymbol simboloVentana = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Windows)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .FirstOrDefault(s =>
                        s.Name.IndexOf(TipoVentana, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.Family.Name.IndexOf(TipoVentana, StringComparison.OrdinalIgnoreCase) >= 0);

                if (simboloVentana == null)
                {
                    simboloVentana = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Windows)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .FirstOrDefault();
                }

                if (simboloVentana == null)
                {
                    Resultado = "Error: No hay ninguna familia de ventanas cargada en este proyecto.";
                    return;
                }

                Wall muroHost = EncontrarMuroMasCercano(doc, puntoVentana, nivelActual);
                if (muroHost == null)
                {
                    Resultado = "Error: No se encontró muro donde colocar la ventana.";
                    return;
                }

                using (Transaction tx = new Transaction(doc, "Colocar Ventana - AI Plugin"))
                {
                    tx.Start();

                    if (!simboloVentana.IsActive)
                    {
                        simboloVentana.Activate();
                        doc.Regenerate();
                    }

                    doc.Create.NewFamilyInstance(puntoVentana, simboloVentana, muroHost, StructuralType.NonStructural);
                    tx.Commit();

                    Resultado = $"✅ Ventana colocada exitosamente: '{simboloVentana.Family.Name} : {simboloVentana.Name}' en X={X}m, Y={Y}m.";
                }
            }
            catch (Exception ex)
            {
                Resultado = $"Error: {ex.Message}";
            }
            finally
            {
                TaskCompletionSource?.TrySetResult(Resultado ?? "Error: Sin respuesta.");
                TaskCompletionSource = null;
            }
        }

        private Level ObtenerNivelActual(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .OrderBy(l => l.Elevation)
                .FirstOrDefault() ?? throw new Exception("No hay niveles en el modelo");
        }

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

                if (muros.Count == 0)
                {
                    muros = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Walls)
                        .OfClass(typeof(Wall))
                        .Cast<Wall>()
                        .ToList();
                }

                if (muros.Count == 0)
                    return null;

                Wall muroMasCercano = null;
                double distanciaMinima = double.MaxValue;

                foreach (var muro in muros)
                {
                    LocationCurve locCurve = muro.Location as LocationCurve;
                    if (locCurve != null)
                    {
                        XYZ proyectado = locCurve.Curve.Project(punto).XYZPoint;
                        double distancia = punto.DistanceTo(proyectado);

                        if (distancia < distanciaMinima)
                        {
                            distanciaMinima = distancia;
                            muroMasCercano = muro;
                        }
                    }
                }

                return muroMasCercano;
            }
            catch
            {
                return null;
            }
        }

        public string GetName() => "ColocarVentanaHandler";
    }
}
