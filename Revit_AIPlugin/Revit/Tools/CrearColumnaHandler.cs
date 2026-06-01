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
    /// Handler para crear columnas estructurales rectangulares en el modelo de Revit.
    /// </summary>
    public class CrearColumnaHandler : IExternalEventHandler
    {
        public string Nivel { get; set; } = "Level 1";
        public double X { get; set; } = 0.0;
        public double Y { get; set; } = 0.0;
        public double Altura { get; set; } = 3.0;
        public string TipoColumna { get; set; } = "";
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

                FamilySymbol tipoColumna = null;

                if (!string.IsNullOrEmpty(TipoColumna))
                {
                    tipoColumna = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralColumns)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .FirstOrDefault(fs =>
                            fs.Name.IndexOf(TipoColumna, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            fs.Family.Name.IndexOf(TipoColumna, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (tipoColumna == null)
                {
                    tipoColumna = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralColumns)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .FirstOrDefault();
                }

                if (tipoColumna == null)
                {
                    Resultado = "Error: No se encontraron familias de columnas en el proyecto.";
                    return;
                }

                double xPies = UnitUtils.ConvertToInternalUnits(X, UnitTypeId.Meters);
                double yPies = UnitUtils.ConvertToInternalUnits(Y, UnitTypeId.Meters);
                double alturaPies = UnitUtils.ConvertToInternalUnits(Altura > 0 ? Altura : 3.0, UnitTypeId.Meters);

                XYZ puntoColumna = new XYZ(xPies, yPies, nivel.Elevation);

                using (Transaction tx = new Transaction(doc, "Crear Columna - AI Plugin"))
                {
                    tx.Start();

                    if (!tipoColumna.IsActive)
                    {
                        tipoColumna.Activate();
                        doc.Regenerate();
                    }

                    FamilyInstance columnaCreada = doc.Create.NewFamilyInstance(
                        puntoColumna,
                        tipoColumna,
                        nivel,
                        StructuralType.Column);

                    if (columnaCreada != null)
                    {
                        Parameter paramAltura = columnaCreada.LookupParameter("Height");
                        if (paramAltura != null && paramAltura.StorageType == StorageType.Double)
                        {
                            paramAltura.Set(alturaPies);
                        }
                    }

                    tx.Commit();

                    Resultado = $"✅ Columna creada con éxito.\n" +
                                $"• Tipo: {tipoColumna.Name}\n" +
                                $"• Nivel: {nivel.Name}\n" +
                                $"• Posición: X={X}m, Y={Y}m\n" +
                                $"• Altura: {Altura}m";
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

        public string GetName() => "CrearColumnaHandler";
    }
}
