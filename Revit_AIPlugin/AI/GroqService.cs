using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Revit_AIPlugin.AI
{
    // Servicio ligero que llama a la API de Groq (o similar) y soporta devoluciones tipo tool-calling.
    public class GroqService
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

        public async Task<string> ChatCompletionAsync(string systemPrompt, string userMessage)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 500,
                // Proporcionar información sobre las tools disponibles para que la IA pueda decidir llamarlas
                tools = new[]
                {
                    new { name = ToolDefinitions.CrearMuro.Name, description = ToolDefinitions.CrearMuro.Description, json_schema = ToolDefinitions.CrearMuro.JsonSchema },
                    new { name = ToolDefinitions.LeerElementos.Name, description = ToolDefinitions.LeerElementos.Description, json_schema = ToolDefinitions.LeerElementos.JsonSchema }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await _http.PostAsync($"{_baseUrl}/v1/chat/completions", content);
            var respJson = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new Exception($"API Error {resp.StatusCode}: {respJson}");

            using var doc = JsonDocument.Parse(respJson);
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString() ?? string.Empty;
        }
    }
}
