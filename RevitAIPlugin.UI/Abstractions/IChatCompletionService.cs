using System.Threading.Tasks;

namespace RevitAIPlugin.UI
{
    public interface IChatCompletionService
    {
        Task<GroqCompletionResult> ChatCompletionAsync(string systemPrompt, string userMessage);
        Task<string> GenerateNaturalResponseAsync(string systemPrompt, string userMessage, string toolResult);
    }
}
