using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text.Json;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Handler que se ejecuta en el hilo principal de Revit para leer elementos.
    /// </summary>
    public class LeerElementosHandler : IExternalEventHandler
    {
        public string Categoria { get; set; }
        public string Resultado { get; set; }

        public void Execute(UIApplication app)
        {
            try
            {
                Document doc = app.ActiveUIDocument.Document;

                var collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElements();

                if (!string.IsNullOrEmpty(Categoria))
                {
                    collector = collector.Where(e => e.Category != null && e.Category.Name.Equals(Categoria, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                var names = collector.Take(50).Select(e => e.Name ?? e.Id.ToString()).ToArray();

                Resultado = JsonSerializer.Serialize(new { count = names.Length, sample = names });
            }
            catch (Exception ex)
            {
                Resultado = $"Error LeerElementosHandler: {ex.Message}";
            }
        }

        public string GetName() => "LeerElementosHandler";
    }
}
