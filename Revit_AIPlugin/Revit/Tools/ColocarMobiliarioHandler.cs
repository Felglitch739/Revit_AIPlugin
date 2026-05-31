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
    /// Coloca una familia de mobiliario en el modelo si existe cargada.
    /// </summary>
    public class ColocarMobiliarioHandler : IExternalEventHandler
    {
        public string TipoMueble { get; set; } = string.Empty;
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
                RevitAILogger.Info("Iniciando ColocarMobiliario: TipoMueble={TipoMueble}, X={X}m, Y={Y}m",
                    TipoMueble, X, Y);

                Document doc = app.ActiveUIDocument.Document;
                XYZ puntoColocacion = new XYZ(
                    UnitUtils.ConvertToInternalUnits(X, UnitTypeId.Meters),
                    UnitUtils.ConvertToInternalUnits(Y, UnitTypeId.Meters),
                    0);

                RevitAILogger.Debug("Punto convertido a unidades internas: {Punto}", puntoColocacion);

                FamilySymbol simbolo = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Furniture)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .FirstOrDefault(s =>
                        s.Name.IndexOf(TipoMueble, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.Family.Name.IndexOf(TipoMueble, StringComparison.OrdinalIgnoreCase) >= 0);

                if (simbolo == null)
                {
                    RevitAILogger.Warn("No se encontró familia de mueble con tipo '{TipoMueble}'. Usando fallback.", TipoMueble);

                    simbolo = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Furniture)
                        .OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .FirstOrDefault();
                }

                if (simbolo == null)
                {
                    RevitAILogger.Error(null, "No hay ninguna familia de mobiliario cargada en el proyecto");
                    Resultado = "Error: No hay ninguna familia de mobiliario cargada en este proyecto.";
                    return;
                }

                RevitAILogger.Debug("Familia encontrada: {FamilyName}:{SymbolName}",
                    simbolo.Family.Name, simbolo.Name);

                using Transaction tx = new Transaction(doc, "Colocar Mobiliario IA");
                tx.Start();

                if (!simbolo.IsActive)
                {
                    simbolo.Activate();
                    doc.Regenerate();
                }

                doc.Create.NewFamilyInstance(puntoColocacion, simbolo, StructuralType.NonStructural);

                tx.Commit();

                stopwatch.Stop();
                RevitAILogger.Info("✅ Mobiliario colocado exitosamente: {Family}:{Symbol} en X={X}m, Y={Y}m (Duracion: {Ms}ms)",
                    simbolo.Family.Name, simbolo.Name, X, Y, stopwatch.ElapsedMilliseconds);

                Resultado = $"✅ Mobiliario colocado: '{simbolo.Family.Name} : {simbolo.Name}' en X={X}m, Y={Y}m.";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                RevitAILogger.Error(ex, "Error al colocar mobiliario (Duracion: {Ms}ms)", stopwatch.ElapsedMilliseconds);
                Resultado = $"Error Revit: {ex.GetType().Name} - {ex.Message}";
            }
            finally
            {
                stopwatch.Stop();
                TaskCompletionSource?.TrySetResult(Resultado ?? "Error: Sin respuesta del handler.");
                TaskCompletionSource = null;
            }
        }

        public string GetName() => "ColocarMobiliarioHandler";
    }
}
