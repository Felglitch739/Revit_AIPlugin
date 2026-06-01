#nullable disable
using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Clase base abstracta para todos los handlers de Revit.
    /// Proporciona manejo de errores centralizado sin dependencias de logging externo.
    /// </summary>
    public abstract class BaseRevitHandler : IRevitHandler
    {
        public string Resultado { get; set; } = null;
        public TaskCompletionSource<string> TaskCompletionSource { get; set; }

        public void Execute(UIApplication app)
        {
            Resultado = null;
            try
            {
                ExecuteInternal(app);
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

        /// <summary>
        /// Implementado por las clases derivadas para ejecutar la lógica específica.
        /// </summary>
        protected abstract void ExecuteInternal(UIApplication app);

        public abstract string GetName();
    }
}
