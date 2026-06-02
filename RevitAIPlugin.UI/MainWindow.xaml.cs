using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RevitAIPlugin.UI
{
    public class ChatMessage
    {
        public string Text { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string Background { get; set; } = "#313244";
        public string TextColor { get; set; } = "#CDD6F4";
        public string Alignment { get; set; } = "Left";
    }

    public partial class MainWindow : Window
    {
        private const string BASE_URL = "https://api.groq.com/openai";
        private const string MODEL = "llama-3.3-70b-versatile";
        private const string API_KEY_ENVIRONMENT_VARIABLE = "GROQ_API_KEY";

        private readonly ObservableCollection<ChatMessage> _messages = new();
        private readonly IChatCompletionService? _chatService;
        private readonly IToolExecutor? _toolExecutor;
        private readonly RevitTaskHandler _revitTaskHandler;
        private int _indiceMensajeProgreso = -1;

        private static string BuildSystemPrompt() => PromptTemplates.BuildSystemPrompt();

        public MainWindow(object? revitDispatcher)
        {
            InitializeComponent();
            messagesPanel.ItemsSource = _messages;
            _chatService = CrearChatService();
            _revitTaskHandler = new RevitTaskHandler();
            _toolExecutor = new RevitToolExecutor(revitDispatcher, _revitTaskHandler);

            AgregarMensaje("¡Hola! Soy tu asistente de Revit. ¿Qué quieres hacer hoy?",
                           "🤖 IA", "#313244", "#CDD6F4", "Left");
        }

        public MainWindow() : this(null)
        {
        }

        private static IChatCompletionService? CrearChatService()
        {
            string? apiKey = Environment.GetEnvironmentVariable(API_KEY_ENVIRONMENT_VARIABLE);
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show(
                    "No se encontró la variable de entorno GROQ_API_KEY. Configúrala y reinicia Visual Studio para usar el asistente.",
                    "Revit AI Assistant",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return null;
            }

            return new GroqService(apiKey, BASE_URL, MODEL);
        }

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
            if (_chatService == null)
            {
                return;
            }

            if (_toolExecutor == null)
            {
                return;
            }

            string userText = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userText))
            {
                return;
            }

            AgregarMensaje(userText, "👤 Tú", "#45475A", "#CDD6F4", "Right");
            txtInput.Text = string.Empty;
            btnEnviar.IsEnabled = false;
            AgregarMensaje("⏳ Preparando ejecución...", "🤖 IA", "#313244", "#6C7086", "Left");
            _indiceMensajeProgreso = _messages.Count - 1;

            try
            {
                GroqCompletionResult completion = await _chatService.ChatCompletionAsync(BuildSystemPrompt(), userText);
                string respuestaFinal = completion.HasToolCalls
                    ? await EjecutarToolsAsync(completion, userText)
                    : string.IsNullOrWhiteSpace(completion.Content)
                        ? "No he podido generar una respuesta útil en este momento."
                        : completion.Content;

                if (_indiceMensajeProgreso >= 0 && _indiceMensajeProgreso < _messages.Count)
                {
                    _messages.RemoveAt(_indiceMensajeProgreso);
                }

                _indiceMensajeProgreso = -1;
                AgregarMensaje(respuestaFinal, "🤖 IA", "#313244", "#CDD6F4", "Left");
            }
            catch (Exception ex)
            {
                if (_indiceMensajeProgreso >= 0 && _indiceMensajeProgreso < _messages.Count)
                {
                    _messages.RemoveAt(_indiceMensajeProgreso);
                }

                _indiceMensajeProgreso = -1;
                AgregarMensaje($"Error: {ex.Message}", "⚠️ Sistema", "#45475A", "#F38BA8", "Left");
            }
            finally
            {
                btnEnviar.IsEnabled = true;
            }
        }

        private async Task<string> EjecutarToolsAsync(GroqCompletionResult completion, string userMessage)
        {
            if (_toolExecutor == null)
            {
                return "Error: el ejecutor de herramientas no está disponible en esta sesión.";
            }

            var resumenHerramientas = new System.Text.StringBuilder();
            var totalPorHerramienta = CalcularTotalPorHerramienta(completion.ToolCalls);
            var ejecutadasPorHerramienta = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (GroqToolCall toolCall in completion.ToolCalls)
            {
                try
                {
                    int ejecutadaActual = ejecutadasPorHerramienta.TryGetValue(toolCall.Name, out int actual) ? actual + 1 : 1;
                    ejecutadasPorHerramienta[toolCall.Name] = ejecutadaActual;
                    int total = totalPorHerramienta.TryGetValue(toolCall.Name, out int totalTool) ? totalTool : 1;

                    ActualizarMensajeProgreso(CrearTextoProgreso(toolCall, ejecutadaActual, total));

                    string toolResult = await _toolExecutor.ExecuteAsync(toolCall);
                    resumenHerramientas.AppendLine(toolResult);
                    resumenHerramientas.AppendLine();

                    await Task.Delay(180);
                }
                catch (Exception ex)
                {
                    resumenHerramientas.AppendLine($"⚠️ {toolCall.Name}: {ex.Message}");
                    resumenHerramientas.AppendLine();
                }
            }

            ActualizarMensajeProgreso("✅ Listo!");

            string toolResultCombined = resumenHerramientas.ToString().Trim();
            if (string.IsNullOrWhiteSpace(toolResultCombined))
            {
                return "No se obtuvo resultado de la herramienta.";
            }

            string naturalResponse = await _chatService!.GenerateNaturalResponseAsync(BuildSystemPrompt(), userMessage, toolResultCombined);
            return string.IsNullOrWhiteSpace(naturalResponse)
                ? toolResultCombined
                : naturalResponse;
        }

        private Dictionary<string, int> CalcularTotalPorHerramienta(IReadOnlyList<GroqToolCall> toolCalls)
        {
            var resultado = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var call in toolCalls)
            {
                resultado[call.Name] = resultado.TryGetValue(call.Name, out int total) ? total + 1 : 1;
            }

            return resultado;
        }

        private void ActualizarMensajeProgreso(string texto)
        {
            if (_indiceMensajeProgreso < 0 || _indiceMensajeProgreso >= _messages.Count)
            {
                return;
            }

            _messages[_indiceMensajeProgreso] = new ChatMessage
            {
                Text = texto,
                Sender = "🤖 IA",
                Background = "#313244",
                TextColor = "#89B4FA",
                Alignment = "Left"
            };
            scrollViewer.ScrollToEnd();
        }

        private string CrearTextoProgreso(GroqToolCall toolCall, int indiceActual, int total)
        {
            using var doc = string.IsNullOrWhiteSpace(toolCall.ArgumentsJson)
                ? null
                : JsonDocument.Parse(toolCall.ArgumentsJson);

            JsonElement root = doc?.RootElement ?? default;

            return toolCall.Name switch
            {
                "CrearHabitacionEstructurada" => $"🏠 Creando habitación {ObtenerDouble(root, "ancho"):0.##}x{ObtenerDouble(root, "largo"):0.##}m...",
                "ColocarVentana" => $"🪟 Colocando ventana {indiceActual} de {total}...",
                "ColocarPuerta" => $"🚪 Colocando puerta {indiceActual} de {total}...",
                "ColocarMobiliario" => $"🛏 Colocando {NormalizarMueble(ObtenerString(root, "tipoMueble"))}...",
                "CrearMuro" => "🧱 Creando muro...",
                "LeerElementos" => "🔎 Leyendo elementos...",
                "CrearColumna" => "🏗️ Creando columna...",
                "CrearViga" => "🏗️ Creando viga...",
                "CrearTecho" => "🏠 Creando techo...",
                _ => $"⚙️ Ejecutando {toolCall.Name}..."
            };
        }

        private static string ObtenerString(JsonElement root, string propertyName)
        {
            return root.ValueKind != JsonValueKind.Undefined && root.TryGetProperty(propertyName, out JsonElement value)
                ? value.GetString() ?? string.Empty
                : string.Empty;
        }

        private static double ObtenerDouble(JsonElement root, string propertyName)
        {
            return root.ValueKind != JsonValueKind.Undefined && root.TryGetProperty(propertyName, out JsonElement value) && value.TryGetDouble(out double numero)
                ? numero
                : 0;
        }

        private static string NormalizarMueble(string tipoMueble)
        {
            if (string.IsNullOrWhiteSpace(tipoMueble))
            {
                return "mobiliario";
            }

            return tipoMueble.Trim().ToLowerInvariant() switch
            {
                "bed-standard" => "cama standard",
                "bed-shaker" => "cama shaker",
                "bed-box" => "cama box",
                _ => tipoMueble
            };
        }
    }
}
