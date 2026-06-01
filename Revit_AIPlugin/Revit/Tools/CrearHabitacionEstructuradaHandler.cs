#nullable disable
using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Crea una habitación rectangular mediante cuatro muros perimetrales.
    /// </summary>
    public class CrearHabitacionEstructuradaHandler : IExternalEventHandler, IHandlerConTCS
    {
        public string Nivel { get; set; } = "Level 1";
        public double Ancho { get; set; } = 4.0;
        public double Largo { get; set; } = 5.0;
        public double Altura { get; set; } = 3.0;
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

                WallType wallType = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType))
                    .Cast<WallType>()
                    .FirstOrDefault();

                if (wallType == null)
                {
                    Resultado = "Error: No se encontraron tipos de muro en el proyecto.";
                    return;
                }

                double anchoPies = UnitUtils.ConvertToInternalUnits(Ancho, UnitTypeId.Meters);
                double largoPies = UnitUtils.ConvertToInternalUnits(Largo, UnitTypeId.Meters);
                double alturaPies = UnitUtils.ConvertToInternalUnits(Altura > 0 ? Altura : 3.0, UnitTypeId.Meters);

                XYZ p1 = new XYZ(0, 0, nivel.Elevation);
                XYZ p2 = new XYZ(anchoPies, 0, nivel.Elevation);
                XYZ p3 = new XYZ(anchoPies, largoPies, nivel.Elevation);
                XYZ p4 = new XYZ(0, largoPies, nivel.Elevation);

                Line l1 = Line.CreateBound(p1, p2);
                Line l2 = Line.CreateBound(p2, p3);
                Line l3 = Line.CreateBound(p3, p4);
                Line l4 = Line.CreateBound(p4, p1);

                using (Transaction tx = new Transaction(doc, "Crear Habitación IA"))
                {
                    tx.Start();
                    Wall.Create(doc, l1, wallType.Id, nivel.Id, alturaPies, 0, false, false);
                    Wall.Create(doc, l2, wallType.Id, nivel.Id, alturaPies, 0, false, false);
                    Wall.Create(doc, l3, wallType.Id, nivel.Id, alturaPies, 0, false, false);
                    Wall.Create(doc, l4, wallType.Id, nivel.Id, alturaPies, 0, false, false);
                    tx.Commit();
                }

                Resultado = $"✅ Habitación creada con éxito en '{nivel.Name}'. Ancho: {Ancho}m, Largo: {Largo}m, Altura: {Altura}m.";
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

        public string GetName() => "CrearHabitacionEstructuradaHandler";
    }
}
