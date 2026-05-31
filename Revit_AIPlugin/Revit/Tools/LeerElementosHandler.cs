#nullable disable
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text.Json;
using Revit_AIPlugin.Logging;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Handler que se ejecuta en el hilo principal de Revit para leer elementos.
    /// </summary>
    public class LeerElementosHandler : IExternalEventHandler
    {
        public string Categoria { get; set; } = "Walls";
        public string Resultado { get; set; } = null;
        public TaskCompletionSource<string> TaskCompletionSource { get; set; }

        private static readonly Dictionary<string, BuiltInCategory> _categorias =
            new Dictionary<string, BuiltInCategory>(StringComparer.OrdinalIgnoreCase)
            {
                { "Walls",    BuiltInCategory.OST_Walls    },
                { "Muros",    BuiltInCategory.OST_Walls    },
                { "Doors",    BuiltInCategory.OST_Doors    },
                { "Puertas",  BuiltInCategory.OST_Doors    },
                { "Windows",  BuiltInCategory.OST_Windows  },
                { "Ventanas", BuiltInCategory.OST_Windows  },
                { "Floors",   BuiltInCategory.OST_Floors   },
                { "Pisos",    BuiltInCategory.OST_Floors   },
                { "Levels",   BuiltInCategory.OST_Levels   },
                { "Niveles",  BuiltInCategory.OST_Levels   },
                { "Columns",  BuiltInCategory.OST_Columns  },
                { "Columnas", BuiltInCategory.OST_Columns  },
            };

        public void Execute(UIApplication app)
        {
            var stopwatch = Stopwatch.StartNew();
            Resultado = null;
            try
            {
                RevitAILogger.Info("Iniciando LeerElementos: Categoria={Categoria}", Categoria);

                Document doc = app.ActiveUIDocument.Document;

                if (!_categorias.TryGetValue(Categoria, out BuiltInCategory bic))
                {
                    RevitAILogger.Warn("Categoría '{Categoria}' no reconocida", Categoria);
                    Resultado = $"Error: Categoría '{Categoria}' no reconocida. " +
                                $"Válidas: {string.Join(", ", _categorias.Keys)}.";
                    return;
                }

                RevitAILogger.Debug("Categoría mapeada a: {BuiltInCategory}", bic);

                var elementos = new FilteredElementCollector(doc)
                    .OfCategory(bic)
                    .WhereElementIsNotElementType()
                    .ToList();

                if (!elementos.Any())
                {
                    RevitAILogger.Warn("No se encontraron elementos de categoría '{Categoria}' (Duracion: {Ms}ms)",
                        Categoria, stopwatch.ElapsedMilliseconds);

                    Resultado = $"No se encontraron elementos de tipo '{Categoria}' en el modelo.";
                    return;
                }

                RevitAILogger.Info("✅ Se encontraron {Count} elemento(s) de tipo '{Categoria}' (Duracion: {Ms}ms)",
                    elementos.Count, Categoria, stopwatch.ElapsedMilliseconds);

                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"Se encontraron {elementos.Count} elemento(s) de tipo '{Categoria}':");

                foreach (var elem in elementos.Take(10))
                {
                    string nombre = elem.Name ?? "(sin nombre)";
                    string nivelStr = elem.LookupParameter("Level")?.AsValueString()
                                   ?? elem.LookupParameter("Base Level")?.AsValueString();

                    sb.AppendLine($"- {nombre} (Level: {nivelStr}) - Id: {elem.Id}");

                    RevitAILogger.Debug("  Elemento: {Name}, ID: {ID}, Level: {Level}",
                        nombre, elem.Id.Value, nivelStr ?? "N/A");
                }

                Resultado = sb.ToString();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                RevitAILogger.Error(ex, "Error en LeerElementosHandler (Duracion: {Ms}ms)", stopwatch.ElapsedMilliseconds);
                Resultado = $"Error Revit: {ex.GetType().Name} - {ex.Message}";
            }
            finally
            {
                stopwatch.Stop();
                TaskCompletionSource?.TrySetResult(Resultado ?? "Error: Sin respuesta del handler.");
                TaskCompletionSource = null;
            }
        }

        public string GetName() => "LeerElementosHandler";
    }
}
