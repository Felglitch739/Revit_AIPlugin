using System.Threading.Tasks;

namespace RevitAIPlugin.UI
{
    public interface IToolExecutor
    {
        Task<string> ExecuteAsync(GroqToolCall toolCall);
    }
}
