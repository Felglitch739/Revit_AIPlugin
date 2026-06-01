using System;
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
            AgregarMensaje("...", "🤖 IA", "#313244", "#6C7086", "Left");

            try
            {
                GroqCompletionResult completion = await _chatService.ChatCompletionAsync(BuildSystemPrompt(), userText);
                string respuestaFinal = completion.HasToolCalls
                    ? await EjecutarToolsAsync(completion, userText)
                    : string.IsNullOrWhiteSpace(completion.Content)
                        ? "No he podido generar una respuesta útil en este momento."
                        : completion.Content;

                if (_messages.Count > 0)
                {
                    _messages.RemoveAt(_messages.Count - 1);
                }

                AgregarMensaje(respuestaFinal, "🤖 IA", "#313244", "#CDD6F4", "Left");
            }
            catch (Exception ex)
            {
                if (_messages.Count > 0)
                {
                    _messages.RemoveAt(_messages.Count - 1);
                }

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

            foreach (GroqToolCall toolCall in completion.ToolCalls)
            {
                try
                {
                    // Ejecutamos cada llamada y añadimos un pequeño retardo entre llamadas para evitar solapamiento de transacciones
                    string toolResult = await _toolExecutor.ExecuteAsync(toolCall);
                    resumenHerramientas.AppendLine(toolResult);
                    resumenHerramientas.AppendLine();

                    await Task.Delay(300); // 300ms entre cada ejecución
                }
                catch (Exception ex)
                {
                    resumenHerramientas.AppendLine($"⚠️ {toolCall.Name}: {ex.Message}");
                    resumenHerramientas.AppendLine();
                }
            }

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
    }
}
