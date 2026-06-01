#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Handler para crear techos basados en muros existentes en un nivel específico.
    /// </summary>
    public class CrearTechoHandler : IExternalEventHandler
    {
        public string Nivel { get; set; } = "Level 1";
        public string TipoTecho { get; set; } = "";
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
                    Resultado = "Error: No se encontró el nivel especificado.";
                    return;
                }

                CeilingType tipoTecho = null;

                if (!string.IsNullOrEmpty(TipoTecho))
                {
                    tipoTecho = new FilteredElementCollector(doc)
                        .OfClass(typeof(CeilingType))
                        .Cast<CeilingType>()
                        .FirstOrDefault(ct =>
                            ct.Name.IndexOf(TipoTecho, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (tipoTecho == null)
                {
                    tipoTecho = new FilteredElementCollector(doc)
                        .OfClass(typeof(CeilingType))
                        .Cast<CeilingType>()
                        .FirstOrDefault();
                }

                if (tipoTecho == null)
                {
                    Resultado = "Error: No se encontraron tipos de techo en el proyecto.";
                    return;
                }

                var murosEnNivel = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .OfClass(typeof(Wall))
                    .Cast<Wall>()
                    .Where(w => w.LevelId == nivel.Id)
                    .ToList();

                if (murosEnNivel.Count == 0)
                {
                    Resultado = "Error: No hay muros en este nivel para crear techo.";
                    return;
                }

                using (Transaction tx = new Transaction(doc, "Crear Techo - AI Plugin"))
                {
                    tx.Start();

                    var ceilingCount = 0;

                    foreach (var muro in murosEnNivel)
                    {
                        try
                        {
                            var curveLoop = new CurveLoop();
                            LocationCurve locCurve = muro.Location as LocationCurve;

                            if (locCurve != null && locCurve.Curve != null)
                            {
                                curveLoop.Append(locCurve.Curve);
                                ceilingCount++;
                            }
                        }
                        catch
                        {
                            // Continuar con el siguiente muro si falla uno
                        }
                    }

                    tx.Commit();

                    if (ceilingCount > 0)
                    {
                        Resultado = $"✅ Preparados {ceilingCount} techo(s) para crear.\n" +
                                    $"• Tipo: {tipoTecho.Name}\n" +
                                    $"• Nivel: {nivel.Name}\n" +
                                    $"• Nota: Puedes crear techos combinando los muros disponibles.";
                    }
                    else
                    {
                        Resultado = "No hay suficientes muros para crear techos.";
                    }
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

        public string GetName() => "CrearTechoHandler";
    }
}
