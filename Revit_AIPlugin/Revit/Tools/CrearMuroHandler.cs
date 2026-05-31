#nullable disable
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit_AIPlugin.Logging;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Handler que se ejecuta en el hilo principal de Revit para crear un muro.
    /// </summary>

    public class CrearMuroHandler : IExternalEventHandler
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
            var stopwatch = Stopwatch.StartNew();
            Resultado = null;
            try
            {
                RevitAILogger.Info("Iniciando CrearMuro: Longitud={Longitud}m, Altura={Altura}m, TipoMuro={TipoMuro}, Nivel={Nivel}",
                    Longitud, Altura, TipoMuro, Nivel);

                Document doc = app.ActiveUIDocument.Document;

                // Buscar nivel por nombre
                Level nivel = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .Cast<Level>()
                    .FirstOrDefault(l =>
                        l.Name.Equals(Nivel, StringComparison.OrdinalIgnoreCase) ||
                        l.Name.IndexOf(Nivel, StringComparison.OrdinalIgnoreCase) >= 0);

                // Fallback: primer nivel disponible
                if (nivel == null)
                {
                    RevitAILogger.Warn("No se encontró nivel '{Nivel}'. Usando primer nivel disponible.", Nivel);

                    nivel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Cast<Level>()
                        .OrderBy(l => l.Elevation)
                        .FirstOrDefault();

                    if (nivel == null)
                    {
                        RevitAILogger.Error(null, "No se encontraron niveles en el modelo");
                        Resultado = "Error: No se encontraron niveles en el modelo.";
                        return;
                    }
                }

                RevitAILogger.Debug("Nivel encontrado: {LevelName}", nivel.Name);

                // Buscar tipo de muro
                WallType tipoMuroObj = null;

                if (!string.IsNullOrEmpty(TipoMuro))
                {
                    tipoMuroObj = new FilteredElementCollector(doc)
                        .OfClass(typeof(WallType))
                        .Cast<WallType>()
                        .FirstOrDefault(wt =>
                            wt.Name.IndexOf(TipoMuro, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                // Fallback: primer tipo disponible
                if (tipoMuroObj == null)
                {
                    if (!string.IsNullOrEmpty(TipoMuro))
                        RevitAILogger.Warn("No se encontró tipo de muro '{TipoMuro}'. Usando tipo por defecto.", TipoMuro);

                    tipoMuroObj = new FilteredElementCollector(doc)
                        .OfClass(typeof(WallType))
                        .Cast<WallType>()
                        .FirstOrDefault();
                }

                if (tipoMuroObj == null)
                {
                    RevitAILogger.Error(null, "No se encontraron tipos de muro en el proyecto");
                    Resultado = "Error: No se encontraron tipos de muro en el proyecto.";
                    return;
                }

                RevitAILogger.Debug("Tipo de muro encontrado: {WallTypeName}", tipoMuroObj.Name);

                // Convertir metros → pies (unidad interna de Revit)
                double longitudPies = UnitUtils.ConvertToInternalUnits(Longitud, UnitTypeId.Meters);
                double alturaReal = Altura > 0 ? Altura : 3.0;
                double alturaPies = UnitUtils.ConvertToInternalUnits(alturaReal, UnitTypeId.Meters);

                RevitAILogger.Debug("Dimensiones convertidas: Longitud={LongPies}, Altura={AltPies}",
                    longitudPies, alturaPies);

                XYZ puntoInicio = new XYZ(0, 0, nivel.Elevation);
                XYZ puntoFin = new XYZ(longitudPies, 0, nivel.Elevation);
                Line lineaMuro = Line.CreateBound(puntoInicio, puntoFin);

                RevitAILogger.Debug("Línea de muro creada: P1={P1}, P2={P2}", puntoInicio, puntoFin);

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

                    stopwatch.Stop();
                    RevitAILogger.Info("✅ Muro creado exitosamente. Tipo: {WallType}, Nivel: {Level}, Longitud: {Length}m, Altura: {Height}m, ID: {WallId} (Duracion: {Ms}ms)",
                        tipoMuroObj.Name, nivel.Name, Longitud, alturaReal, muroCreado.Id.Value, stopwatch.ElapsedMilliseconds);

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
                stopwatch.Stop();
                RevitAILogger.Error(ex, "Error al crear muro (Duracion: {Ms}ms)", stopwatch.ElapsedMilliseconds);
                Resultado = $"Error Revit: {ex.GetType().Name} - {ex.Message}";
            }
            finally
            {
                stopwatch.Stop();
                TaskCompletionSource?.TrySetResult(Resultado ?? "Error: Sin respuesta del handler.");
                TaskCompletionSource = null;
            }
        }

        public string GetName() => "CrearMuroHandler";
    }
}
