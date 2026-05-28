using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RevitAIPlugin.UI
{
    public class ChatMessage
    {
        public string Text { get; set; } = "";
        public string Sender { get; set; } = "";
        public string Background { get; set; } = "#313244";
        public string TextColor { get; set; } = "#CDD6F4";
        public string Alignment { get; set; } = "Left";
    }

    public partial class MainWindow : Window
    {
        // ⚠️ IMPORTANTE: Pon aquí tu API key y base URL del hackaton
        private const string API_KEY = "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";
        private const string BASE_URL = "https://api.groq.com/openai";
        private const string MODEL = "llama-3.3-70b-versatile";

        private readonly HttpClient _httpClient = new();
        private readonly ObservableCollection<ChatMessage> _messages = new();
        private readonly object _uiapp;

        // Sistema prompt — le explica a la IA que está dentro de Revit
        private const string SYSTEM_PROMPT = """
            Eres un asistente de IA integrado dentro de Autodesk Revit 2026.
            Ayudas a arquitectos e ingenieros a entender qué pueden hacer con el modelo BIM.
            Responde siempre en español, de forma clara y concisa.
            Si el usuario pide crear elementos, explica qué harías paso a paso.
            Por ahora estamos en modo de prueba — confirma que entendiste la instrucción
            y explica cómo la ejecutarías en Revit.
            """;

        // Nuevo constructor que acepta la instancia de UIApplication como objeto para evitar referencias cruzadas entre proyectos.
        public MainWindow(object uiapp)
        {
            _uiapp = uiapp;
            InitializeComponent();
            messagesPanel.ItemsSource = _messages;

            // Mensaje de bienvenida
            AgregarMensaje("¡Hola! Soy tu asistente de Revit. ¿Qué quieres hacer hoy?",
                           "🤖 IA", "#313244", "#CDD6F4", "Left");
        }

        // Mantener constructor por compatibilidad si alguien lo llama sin UIApplication
        public MainWindow() : this(null) { }

        private void AgregarMensaje(string texto, string sender, string bg, string color, string alignment)
        {
            _messages.Add(new ChatMessage
            {
                Text = texto,
                Sender = sender,
                Background = bg,
                TextColor = color,
                Alignment = alignment
            });

            // Scroll al último mensaje
            scrollViewer.ScrollToEnd();
        }

        private async void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            await EnviarMensaje();
        }

        private async void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift))
            {
                e.Handled = true;
                await EnviarMensaje();
            }
        }

        private async Task EnviarMensaje()
        {
            string userText = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userText)) return;

            // Mostrar mensaje del usuario
            AgregarMensaje(userText, "👤 Tú", "#45475A", "#CDD6F4", "Right");
            txtInput.Text = "";
            btnEnviar.IsEnabled = false;

            // Indicador de "escribiendo..."
            AgregarMensaje("...", "🤖 IA", "#313244", "#6C7086", "Left");

            try
            {
                string respuesta = await LlamarGPT(userText);

                // Quitar el indicador y mostrar respuesta real
                _messages.RemoveAt(_messages.Count - 1);
                AgregarMensaje(respuesta, "🤖 IA", "#313244", "#CDD6F4", "Left");
            }
            catch (Exception ex)
            {
                _messages.RemoveAt(_messages.Count - 1);
                AgregarMensaje($"Error: {ex.Message}", "⚠️ Sistema", "#45475A", "#F38BA8", "Left");
            }
            finally
            {
                btnEnviar.IsEnabled = true;
            }
        }

        private async Task<string> LlamarGPT(string userMessage)
        {
            var requestBody = new
            {
                model = MODEL,
                messages = new[]
                {
                    new { role = "system", content = SYSTEM_PROMPT },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 500
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");

            var response = await _httpClient.PostAsync($"{BASE_URL}/v1/chat/completions", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API Error {response.StatusCode}: {responseJson}");

            using var doc = JsonDocument.Parse(responseJson);
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString() ?? "Sin respuesta";
        }
    }
}