#nullable disable
using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Handler para crear vigas estructurales entre dos puntos en el modelo de Revit.
    /// </summary>
    public class CrearVigaHandler : IExternalEventHandler
    {
        public string Nivel { get; set; } = "Level 1";
        public double X1 { get; set; } = 0.0;
        public double Y1 { get; set; } = 0.0;
        public double X2 { get; set; } = 5.0;
        public double Y2 { get; set; } = 0.0;
        public string TipoViga { get; set; } = "";
        public string Resultado { get; set; } = null;
        public TaskCompletionSource<string> TaskCompletionSource { get; set; }

        public void Execute(UIApplication app)
        {
            Resultado = null;
            try
            {
                Document doc = app.ActiveUIDocument.Document;

                Level nivel = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .Cast<Level>()
                    .FirstOrDefault(l =>
                        l.Name.Equals(Nivel, StringComparison.OrdinalIgnoreCase) ||
                        l.Name.IndexOf(Nivel, StringComparison.OrdinalIgnoreCase) >= 0);

                if (nivel == null)
                {
                    nivel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Cast<Level>()
                        .OrderBy(l => l.Elevation)
                        .FirstOrDefault();
                }

                if (nivel == null)
                {
                    Resultado = "Error: No se encontraron niveles en el modelo.";
                    return;
                }

                FamilySymbol tipoViga = null;

                if (!string.IsNullOrEmpty(TipoViga))
                {
                    tipoViga = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralFraming)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .FirstOrDefault(fs =>
                            fs.Name.IndexOf(TipoViga, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            fs.Family.Name.IndexOf(TipoViga, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (tipoViga == null)
                {
                    tipoViga = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralFraming)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .FirstOrDefault();
                }

                if (tipoViga == null)
                {
                    Resultado = "Error: No se encontraron familias de vigas en el proyecto.";
                    return;
                }

                double x1Pies = UnitUtils.ConvertToInternalUnits(X1, UnitTypeId.Meters);
                double y1Pies = UnitUtils.ConvertToInternalUnits(Y1, UnitTypeId.Meters);
                double x2Pies = UnitUtils.ConvertToInternalUnits(X2, UnitTypeId.Meters);
                double y2Pies = UnitUtils.ConvertToInternalUnits(Y2, UnitTypeId.Meters);

                XYZ puntoPrincipio = new XYZ(x1Pies, y1Pies, nivel.Elevation);
                XYZ puntoFin = new XYZ(x2Pies, y2Pies, nivel.Elevation);
                Line lineaViga = Line.CreateBound(puntoPrincipio, puntoFin);

                using (Transaction tx = new Transaction(doc, "Crear Viga - AI Plugin"))
                {
                    tx.Start();

                    if (!tipoViga.IsActive)
                    {
                        tipoViga.Activate();
                        doc.Regenerate();
                    }

                    FamilyInstance vigaCreada = doc.Create.NewFamilyInstance(
                        lineaViga,
                        tipoViga,
                        nivel,
                        StructuralType.Beam);

                    tx.Commit();

                    double longitud = Math.Sqrt(
                        Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2));

                    Resultado = $"✅ Viga creada con éxito.\n" +
                                $"• Tipo: {tipoViga.Name}\n" +
                                $"• Nivel: {nivel.Name}\n" +
                                $"• Desde: X={X1}m, Y={Y1}m\n" +
                                $"• Hasta: X={X2}m, Y={Y2}m\n" +
                                $"• Longitud: {longitud:F2}m";
                }
            }
            catch (Exception ex)
            {
                Resultado = $"Error: {ex.Message}";
            }
            finally
            {
                TaskCompletionSource?.TrySetResult(Resultado ?? "Error: Sin respuesta del handler.");
                TaskCompletionSource = null;
            }
        }

        public string GetName() => "CrearVigaHandler";
    }
}
