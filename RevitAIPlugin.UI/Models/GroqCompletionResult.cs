using System.Collections.Generic;

namespace RevitAIPlugin.UI
{
    public sealed class GroqCompletionResult
    {
        public string Content { get; init; } = string.Empty;
        public IReadOnlyList<GroqToolCall> ToolCalls { get; init; } = [];

        public bool HasToolCalls => ToolCalls.Count > 0;
    }
}
