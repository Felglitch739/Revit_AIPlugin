#nullable disable
using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Handler que se ejecuta en el hilo principal de Revit para crear un muro.
    /// </summary>
    public class CrearMuroHandler : IExternalEventHandler, IHandlerConTCS
    {
        // Valores por defecto para silenciar CS8618
        public string Nivel { get; set; } = "Level 1";
        public double Longitud { get; set; } = 5.0;
        public double Altura { get; set; } = 3.0;
        public string TipoMuro { get; set; } = "";
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

                    if (nivel == null)
                    {
                        Resultado = "Error: No se encontraron niveles en el modelo.";
                        return;
                    }
                }

                WallType tipoMuroObj = null;

                if (!string.IsNullOrEmpty(TipoMuro))
                {
                    tipoMuroObj = new FilteredElementCollector(doc)
                        .OfClass(typeof(WallType))
                        .Cast<WallType>()
                        .FirstOrDefault(wt =>
                            wt.Name.IndexOf(TipoMuro, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (tipoMuroObj == null)
                {
                    tipoMuroObj = new FilteredElementCollector(doc)
                        .OfClass(typeof(WallType))
                        .Cast<WallType>()
                        .FirstOrDefault();
                }

                if (tipoMuroObj == null)
                {
                    Resultado = "Error: No se encontraron tipos de muro en el proyecto.";
                    return;
                }

                double longitudPies = UnitUtils.ConvertToInternalUnits(Longitud, UnitTypeId.Meters);
                double alturaReal = Altura > 0 ? Altura : 3.0;
                double alturaPies = UnitUtils.ConvertToInternalUnits(alturaReal, UnitTypeId.Meters);

                XYZ puntoInicio = new XYZ(0, 0, nivel.Elevation);
                XYZ puntoFin = new XYZ(longitudPies, 0, nivel.Elevation);
                Line lineaMuro = Line.CreateBound(puntoInicio, puntoFin);

                using (Transaction tx = new Transaction(doc, "Crear Muro - AI Plugin"))
                {
                    tx.Start();

                    Wall muroCreado = Wall.Create(
                        doc,
                        lineaMuro,
                        tipoMuroObj.Id,
                        nivel.Id,
                        alturaPies,
                        0,
                        false,
                        false
                    );

                    tx.Commit();

                    Resultado = $"✅ Muro creado exitosamente.\n" +
                                $"• Tipo: {tipoMuroObj.Name}\n" +
                                $"• Nivel: {nivel.Name}\n" +
                                $"• Longitud: {Longitud}m\n" +
                                $"• Altura: {alturaReal}m\n" +
                                $"• ID del elemento: {muroCreado.Id.Value}";
                }
            }
            catch (Exception ex)
            {
                Resultado = $"ERROR: {ex.Message}";
            }
            finally
            {
                TaskCompletionSource?.TrySetResult(Resultado ?? "ERROR: Sin respuesta.");
                TaskCompletionSource = null;
            }
        }

        public string GetName() => "CrearMuroHandler";
    }
}
