using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text.Json;

namespace Revit_AIPlugin.Revit.Tools
{
    public static class LeerElementosTool
    {
        public static string Run(UIApplication uiapp, string jsonArgs)
        {
            try
            {
                var doc = uiapp?.ActiveUIDocument?.Document;
                if (doc == null) return "Documento no disponible";

                var args = JsonSerializer.Deserialize<LeerElementosArgs>(jsonArgs);
                string category = args?.category;

                // Uso simple de FilteredElementCollector para listar elementos
                var collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElements();

                if (!string.IsNullOrEmpty(category))
                {
                    collector = collector.Where(e => e.Category != null && e.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                var names = collector.Take(20).Select(e => e.Name ?? e.Id.ToString()).ToArray();

                return JsonSerializer.Serialize(new { count = names.Length, sample = names });
            }
            catch (Exception ex)
            {
                return $"Error LeerElementosTool: {ex.Message}";
            }
        }

        private class LeerElementosArgs
        {
            public string category { get; set; }
        }
    }
}
