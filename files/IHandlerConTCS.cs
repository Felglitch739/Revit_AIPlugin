using System.Threading.Tasks;

namespace RevitAIPlugin.Revit.Tools
{
    /// <summary>
    /// Contrato mínimo que debe cumplir todo handler para poder usar
    /// el método Despachar() centralizado del RevitCommandDispatcher.
    /// 
    /// Tu CrearHabitacionEstructuradaHandler ya lo tiene implementado.
    /// Solo asegúrate de que TODOS los demás handlers tengan esta propiedad
    /// y el patrón try/catch/finally con TrySetResult en su Execute().
    /// </summary>
    public interface IHandlerConTCS
    {
        TaskCompletionSource<string> TaskCompletionSource { get; set; }
    }
}
