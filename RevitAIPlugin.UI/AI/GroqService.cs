using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RevitAIPlugin.UI
{
    public class GroqService : IChatCompletionService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _model;

        public GroqService(string apiKey, string baseUrl = "https://api.groq.com/openai", string model = "llama-3.3-70b-versatile")
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl;
            _model = model;
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<GroqCompletionResult> ChatCompletionAsync(string systemPrompt, string userMessage)
        {
            string combinedSystemPrompt = $"{systemPrompt}\n\n{PromptTemplates.BuildToolSelectionHint()}";

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = combinedSystemPrompt },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 500,
                temperature = 0.2,
                tools = new[]
                {
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = ToolDefinitions.CrearMuro.Name,
                            description = ToolDefinitions.CrearMuro.Description,
                            parameters = JsonDocument.Parse(ToolDefinitions.CrearMuro.JsonSchema).RootElement
                        }
                    },
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = ToolDefinitions.LeerElementos.Name,
                            description = ToolDefinitions.LeerElementos.Description,
                            parameters = JsonDocument.Parse(ToolDefinitions.LeerElementos.JsonSchema).RootElement
                        }
                    },
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = ToolDefinitions.CrearHabitacionEstructurada.Name,
                            description = ToolDefinitions.CrearHabitacionEstructurada.Description,
                            parameters = JsonDocument.Parse(ToolDefinitions.CrearHabitacionEstructurada.JsonSchema).RootElement
                        }
                    },
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = ToolDefinitions.CrearColumna.Name,
                            description = ToolDefinitions.CrearColumna.Description,
                            parameters = JsonDocument.Parse(ToolDefinitions.CrearColumna.JsonSchema).RootElement
                        }
                    },
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = ToolDefinitions.CrearViga.Name,
                            description = ToolDefinitions.CrearViga.Description,
                            parameters = JsonDocument.Parse(ToolDefinitions.CrearViga.JsonSchema).RootElement
                        }
                    },
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = ToolDefinitions.CrearTecho.Name,
                            description = ToolDefinitions.CrearTecho.Description,
                            parameters = JsonDocument.Parse(ToolDefinitions.CrearTecho.JsonSchema).RootElement
                        }
                    },
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = ToolDefinitions.ColocarMobiliario.Name,
                            description = ToolDefinitions.ColocarMobiliario.Description,
                            parameters = JsonDocument.Parse(ToolDefinitions.ColocarMobiliario.JsonSchema).RootElement
                        }
                    },
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = ToolDefinitions.ColocarPuerta.Name,
                            description = ToolDefinitions.ColocarPuerta.Description,
                            parameters = JsonDocument.Parse(ToolDefinitions.ColocarPuerta.JsonSchema).RootElement
                        }
                    },
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = ToolDefinitions.ColocarVentana.Name,
                            description = ToolDefinitions.ColocarVentana.Description,
                            parameters = JsonDocument.Parse(ToolDefinitions.ColocarVentana.JsonSchema).RootElement
                        }
                    }
                },
                tool_choice = "auto"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await _http.PostAsync($"{_baseUrl}/v1/chat/completions", content);
            var respJson = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception($"API Error {resp.StatusCode}: {respJson}");
            }

            using var doc = JsonDocument.Parse(respJson);
            var message = doc.RootElement.GetProperty("choices")[0].GetProperty("message");

            return new GroqCompletionResult
            {
                Content = message.TryGetProperty("content", out var contentElement)
                    ? contentElement.GetString() ?? string.Empty
                    : string.Empty,
                ToolCalls = message.TryGetProperty("tool_calls", out var toolCalls)
                    ? ParseToolCalls(toolCalls)
                    : []
            };
        }

        public async Task<string> GenerateNaturalResponseAsync(string systemPrompt, string userMessage, string toolResult)
        {
            string responsePrompt = PromptTemplates.BuildNaturalResponsePrompt(userMessage, toolResult);

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = responsePrompt }
                },
                max_tokens = 220,
                temperature = 0.2,
                tool_choice = "none"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await _http.PostAsync($"{_baseUrl}/v1/chat/completions", content);
            var respJson = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception($"API Error {resp.StatusCode}: {respJson}");
            }

            using var doc = JsonDocument.Parse(respJson);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
        }

        private static IReadOnlyList<GroqToolCall> ParseToolCalls(JsonElement toolCalls)
        {
            var calls = new List<GroqToolCall>();

            foreach (var toolCall in toolCalls.EnumerateArray())
            {
                var id = toolCall.TryGetProperty("id", out var idElement) ? idElement.GetString() ?? string.Empty : string.Empty;
                var function = toolCall.GetProperty("function");
                var name = function.TryGetProperty("name", out var nameElement) ? nameElement.GetString() ?? string.Empty : string.Empty;
                var arguments = function.TryGetProperty("arguments", out var argumentsElement)
                    ? argumentsElement.GetString() ?? string.Empty
                    : string.Empty;

                calls.Add(new GroqToolCall
                {
                    Id = id,
                    Name = name,
                    ArgumentsJson = arguments
                });
            }

            return calls;
        }
    }
}
