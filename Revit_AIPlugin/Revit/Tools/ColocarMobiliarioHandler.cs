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
    /// Handler para colocar mobiliario en el modelo.
    /// </summary>
    public class ColocarMobiliarioHandler : IExternalEventHandler, IHandlerConTCS
    {
        public string TipoMueble { get; set; } = string.Empty;
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
                XYZ puntoPies = new XYZ(
                    UnitUtils.ConvertToInternalUnits(X, UnitTypeId.Meters),
                    UnitUtils.ConvertToInternalUnits(Y, UnitTypeId.Meters),
                    0);

                Level nivel = app.ActiveUIDocument.ActiveView?.GenLevel;
                if (nivel == null)
                {
                    nivel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Cast<Level>()
                        .FirstOrDefault(l =>
                            l.Name.Equals("L1", StringComparison.OrdinalIgnoreCase) ||
                            l.Name.Equals("Level 1", StringComparison.OrdinalIgnoreCase) ||
                            l.Name.IndexOf("L1", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            l.Name.IndexOf("Level 1", StringComparison.OrdinalIgnoreCase) >= 0);
                }

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
                    Resultado = "Error: No hay niveles disponibles en el proyecto.";
                    return;
                }

                FamilySymbol simbolo = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Furniture)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .FirstOrDefault(s =>
                        s.Name.IndexOf(TipoMueble, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.Family.Name.IndexOf(TipoMueble, StringComparison.OrdinalIgnoreCase) >= 0);

                if (simbolo == null)
                {
                    simbolo = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Furniture)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .FirstOrDefault();
                }

                if (simbolo == null)
                {
                    Resultado = "Error: No hay ninguna familia de mobiliario cargada en este proyecto.";
                    return;
                }

                using (Transaction tx = new Transaction(doc, "Colocar Mobiliario IA"))
                {
                    tx.Start();

                    if (!simbolo.IsActive)
                    {
                        simbolo.Activate();
                        doc.Regenerate();
                    }

                    doc.Create.NewFamilyInstance(puntoPies, simbolo, nivel, StructuralType.NonStructural);
                    tx.Commit();

                    Resultado = $"✅ Mobiliario colocado: '{simbolo.Family.Name} : {simbolo.Name}' en X={X}m, Y={Y}m.";
                }
            }
            catch (Exception ex)
            {
                Resultado = ex.Message;
            }
            finally
            {
                TaskCompletionSource?.TrySetResult(Resultado ?? "Error: Sin respuesta.");
                TaskCompletionSource = null;
            }
        }

        public string GetName() => "ColocarMobiliarioHandler";
    }
}
