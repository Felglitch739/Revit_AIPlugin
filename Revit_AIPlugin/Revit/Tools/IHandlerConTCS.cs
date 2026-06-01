using System.Threading.Tasks;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Contrato mínimo que debe cumplir todo handler para poder usar
    /// un despacho centralizado con TaskCompletionSource.
    /// </summary>
    public interface IHandlerConTCS
    {
        TaskCompletionSource<string> TaskCompletionSource { get; set; }
    }
}
