namespace RevitAIPlugin.UI
{
    public sealed class GroqToolCall
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string ArgumentsJson { get; init; } = string.Empty;
    }
}
