using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Handler que se ejecuta en el hilo principal de Revit para crear un muro.
    /// </summary>
    public class CrearMuroHandler : IExternalEventHandler
    {
        // Parámetros de entrada (seteados antes de Raise())
        public string Nivel { get; set; }
        public double Longitud { get; set; }
        public double Altura { get; set; }
        public string TipoMuro { get; set; }

        // Resultado de salida (leído después de que Execute termine)
        public string Resultado { get; set; }

        public void Execute(UIApplication app)
        {
            try
            {
                Document doc = app.ActiveUIDocument.Document;

                // 1. Buscar el nivel por nombre (case-insensitive)
                Level nivel = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .Cast<Level>()
                    .FirstOrDefault(l =>
                        l.Name.Equals(Nivel, StringComparison.OrdinalIgnoreCase) ||
                        l.Name.Contains(Nivel, StringComparison.OrdinalIgnoreCase));

                if (nivel == null)
                {
                    // Si no se encuentra, usar el primer nivel disponible
                    nivel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Cast<Level>()
                        .OrderBy(l => l.Elevation)
                        .FirstOrDefault();

                    if (nivel == null)
                    {
                        Resultado = $"Error: No se encontraron niveles en el modelo.";
                        return;
                    }
                }

                // 2. Buscar el tipo de muro
                WallType tipoMuroObj = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType))
                    .Cast<WallType>()
                    .FirstOrDefault(wt =>
                        !string.IsNullOrEmpty(TipoMuro) &&
                        wt.Name.Contains(TipoMuro, StringComparison.OrdinalIgnoreCase));

                // Si no se encuentra el tipo específico, usar el primero disponible
                if (tipoMuroObj == null)
                {
                    tipoMuroObj = new FilteredElementCollector(doc)
                        .OfClass(typeof(WallType))
                        .Cast<WallType>()
                        .FirstOrDefault();
                }

                if (tipoMuroObj == null)
                {
                    Resultado = "Error: No se encontraron tipos de muro en el documento.";
                    return;
                }

                // 3. Crear el muro (línea simple en el nivel encontrado)
                using (Transaction t = new Transaction(doc, "Crear Muro desde IA"))
                {
                    t.Start();

                    // Crear una línea recta en el origen con la longitud solicitada
                    XYZ p1 = new XYZ(0, 0, 0);
                    XYZ p2 = new XYZ(Longitud, 0, 0);
                    Line line = Line.CreateBound(p1, p2);

                    Wall wall = Wall.Create(doc, line, tipoMuroObj.Id, nivel.Id, Altura, 0, false, false);

                    t.Commit();
                }

                Resultado = $"Muro creado: longitud={Longitud}m, altura={Altura}m, tipo={(tipoMuroObj?.Name ?? "(default)")}";
            }
            catch (Exception ex)
            {
                Resultado = $"Error CrearMuroHandler: {ex.Message}";
            }
        }

        public string GetName() => "CrearMuroHandler";
    }
}
