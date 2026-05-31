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
    /// Crea una habitación rectangular mediante cuatro muros perimetrales.
    /// </summary>
    public class CrearHabitacionEstructuradaHandler : IExternalEventHandler
    {
        public string Nivel { get; set; } = "Level 1";
        public double Ancho { get; set; } = 4.0;
        public double Largo { get; set; } = 5.0;
        public double Altura { get; set; } = 3.0;
        public string Resultado { get; set; } = null;
        public TaskCompletionSource<string> TaskCompletionSource { get; set; }

        public void Execute(UIApplication app)
        {
            var stopwatch = Stopwatch.StartNew();
            Resultado = null;
            try
            {
                RevitAILogger.Info("Iniciando CrearHabitacionEstructurada: Ancho={Ancho}m, Largo={Largo}m, Altura={Altura}m, Nivel={Nivel}",
                    Ancho, Largo, Altura, Nivel);

                Document doc = app.ActiveUIDocument.Document;

                Level nivel = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .Cast<Level>()
                    .FirstOrDefault(l =>
                        l.Name.Equals(Nivel, StringComparison.OrdinalIgnoreCase) ||
                        l.Name.IndexOf(Nivel, StringComparison.OrdinalIgnoreCase) >= 0);

                if (nivel == null)
                {
                    RevitAILogger.Warn("No se encontró nivel '{Nivel}'. Usando nivel base.", Nivel);

                    nivel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Cast<Level>()
                        .OrderBy(l => l.Elevation)
                        .FirstOrDefault();
                }

                if (nivel == null)
                {
                    RevitAILogger.Error(null, "No se encontraron niveles en el modelo");
                    Resultado = "Error: No se encontraron niveles en el modelo.";
                    return;
                }

                RevitAILogger.Debug("Nivel encontrado: {LevelName} (Elevation: {Elevation}m)",
                    nivel.Name, UnitUtils.ConvertFromInternalUnits(nivel.Elevation, UnitTypeId.Meters));

                WallType wallType = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType))
                    .Cast<WallType>()
                    .FirstOrDefault();

                if (wallType == null)
                {
                    RevitAILogger.Error(null, "No se encontraron tipos de muro en el proyecto");
                    Resultado = "Error: No se encontraron tipos de muro en el proyecto.";
                    return;
                }

                RevitAILogger.Debug("Tipo de muro encontrado: {WallTypeName}", wallType.Name);

                double anchoPies = UnitUtils.ConvertToInternalUnits(Ancho, UnitTypeId.Meters);
                double largoPies = UnitUtils.ConvertToInternalUnits(Largo, UnitTypeId.Meters);
                double alturaPies = UnitUtils.ConvertToInternalUnits(Altura > 0 ? Altura : 3.0, UnitTypeId.Meters);

                RevitAILogger.Debug("Dimensiones convertidas a unidades internas: Ancho={AnchoPies}, Largo={LargoPies}, Altura={AlturaPies}",
                    anchoPies, largoPies, alturaPies);

                XYZ p1 = new XYZ(0, 0, nivel.Elevation);
                XYZ p2 = new XYZ(anchoPies, 0, nivel.Elevation);
                XYZ p3 = new XYZ(anchoPies, largoPies, nivel.Elevation);
                XYZ p4 = new XYZ(0, largoPies, nivel.Elevation);

                Line l1 = Line.CreateBound(p1, p2);
                Line l2 = Line.CreateBound(p2, p3);
                Line l3 = Line.CreateBound(p3, p4);
                Line l4 = Line.CreateBound(p4, p1);

                RevitAILogger.Debug("Líneas de muros creadas para la habitación rectangular");

                using Transaction tx = new Transaction(doc, "Crear Habitación IA");
                tx.Start();

                Wall.Create(doc, l1, wallType.Id, nivel.Id, alturaPies, 0, false, false);
                Wall.Create(doc, l2, wallType.Id, nivel.Id, alturaPies, 0, false, false);
                Wall.Create(doc, l3, wallType.Id, nivel.Id, alturaPies, 0, false, false);
                Wall.Create(doc, l4, wallType.Id, nivel.Id, alturaPies, 0, false, false);

                tx.Commit();

                stopwatch.Stop();
                RevitAILogger.Info("✅ Habitación creada exitosamente en '{Level}'. Ancho: {Ancho}m, Largo: {Largo}m, Altura: {Altura}m (Duracion: {Ms}ms)",
                    nivel.Name, Ancho, Largo, Altura, stopwatch.ElapsedMilliseconds);

                Resultado = $"✅ Habitación creada con éxito en '{nivel.Name}'. Ancho: {Ancho}m, Largo: {Largo}m, Altura: {Altura}m.";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                RevitAILogger.Error(ex, "Error al crear la habitación (Duracion: {Ms}ms)", stopwatch.ElapsedMilliseconds);
                Resultado = $"Error Revit: {ex.GetType().Name} - {ex.Message}";
            }
            finally
            {
                stopwatch.Stop();
                TaskCompletionSource?.TrySetResult(Resultado ?? "Error: Sin respuesta del handler.");
                TaskCompletionSource = null;
            }
        }

        public string GetName() => "CrearHabitacionEstructuradaHandler";
    }
}
