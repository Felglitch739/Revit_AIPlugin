#nullable disable
using Autodesk.Revit.UI;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Interfaz base para todos los handlers de Revit.
    /// </summary>
    public interface IRevitHandler : IExternalEventHandler
    {
        string Resultado { get; set; }
        System.Threading.Tasks.TaskCompletionSource<string> TaskCompletionSource { get; set; }
    }
}
