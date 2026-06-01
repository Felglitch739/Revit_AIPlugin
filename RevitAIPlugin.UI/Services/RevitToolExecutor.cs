using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace RevitAIPlugin.UI
{
    internal sealed class RevitToolExecutor : IToolExecutor
    {
        private readonly object? _revitDispatcher;
        private readonly RevitTaskHandler _revitTaskHandler;

        public RevitToolExecutor(object? revitDispatcher, RevitTaskHandler revitTaskHandler)
        {
            _revitDispatcher = revitDispatcher;
            _revitTaskHandler = revitTaskHandler;
        }

        public async Task<string> ExecuteAsync(GroqToolCall toolCall)
        {
            try
            {
                return toolCall.Name switch
                {
                    "LeerElementos" => await ExecuteToolAsync("LeerElementos", ParseLeerElementos(toolCall.ArgumentsJson)),
                    "CrearMuro" => await ExecuteToolAsync("CrearMuro", ParseCrearMuro(toolCall.ArgumentsJson)),
                    "CrearHabitacionEstructurada" => await ExecuteToolAsync("CrearHabitacionEstructurada", ParseCrearHabitacion(toolCall.ArgumentsJson)),
                    "CrearColumna" => await ExecuteToolAsync("CrearColumna", ParseCrearColumna(toolCall.ArgumentsJson)),
                    "CrearViga" => await ExecuteToolAsync("CrearViga", ParseCrearViga(toolCall.ArgumentsJson)),
                    "CrearTecho" => await ExecuteToolAsync("CrearTecho", ParseCrearTecho(toolCall.ArgumentsJson)),
                    "ColocarMobiliario" => await ExecuteToolAsync("ColocarMobiliario", ParseColocarMobiliario(toolCall.ArgumentsJson)),
                    "ColocarPuerta" => await ExecuteToolAsync("ColocarPuerta", ParseColocarPuerta(toolCall.ArgumentsJson)),
                    "ColocarVentana" => await ExecuteToolAsync("ColocarVentana", ParseColocarVentana(toolCall.ArgumentsJson)),
                    _ => $"Error: herramienta no soportada '{toolCall.Name}'."
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private object[] ParseLeerElementos(string argumentsJson)
        {
            string categoria = "Walls";
            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                if (doc.RootElement.TryGetProperty("categoria", out JsonElement categoriaElement))
                {
                    categoria = categoriaElement.GetString() ?? categoria;
                }
            }

            return new object[] { categoria };
        }

        private object[] ParseCrearMuro(string argumentsJson)
        {
            string nivel = "Level 1";
            string tipoMuro = string.Empty;
            double longitud = 5.0;
            double altura = 3.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("nivel", out JsonElement nivelElement)) nivel = nivelElement.GetString() ?? nivel;
                if (root.TryGetProperty("tipoMuro", out JsonElement tipoMuroElement)) tipoMuro = tipoMuroElement.GetString() ?? string.Empty;
                if (root.TryGetProperty("longitud", out JsonElement longitudElement) && longitudElement.TryGetDouble(out double longitudValue)) longitud = longitudValue;
                if (root.TryGetProperty("altura", out JsonElement alturaElement) && alturaElement.TryGetDouble(out double alturaValue)) altura = alturaValue;
            }

            return new object[] { nivel, longitud, altura, tipoMuro };
        }

        private object[] ParseCrearHabitacion(string argumentsJson)
        {
            string nivel = "Level 1";
            double ancho = 4.0;
            double largo = 5.0;
            double altura = 3.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("nivel", out JsonElement nivelElement)) nivel = nivelElement.GetString() ?? nivel;
                if (root.TryGetProperty("ancho", out JsonElement anchoElement) && anchoElement.TryGetDouble(out double anchoValue)) ancho = anchoValue;
                if (root.TryGetProperty("largo", out JsonElement largoElement) && largoElement.TryGetDouble(out double largoValue)) largo = largoValue;
                if (root.TryGetProperty("altura", out JsonElement alturaElement) && alturaElement.TryGetDouble(out double alturaValue)) altura = alturaValue;
            }

            return new object[] { nivel, ancho, largo, altura };
        }

        private object[] ParseColocarMobiliario(string argumentsJson)
        {
            string tipoMueble = string.Empty;
            double x = 0.0;
            double y = 0.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("tipoMueble", out JsonElement tipoMuebleElement)) tipoMueble = tipoMuebleElement.GetString() ?? string.Empty;
                if (root.TryGetProperty("x", out JsonElement xElement) && xElement.TryGetDouble(out double xValue)) x = xValue;
                if (root.TryGetProperty("y", out JsonElement yElement) && yElement.TryGetDouble(out double yValue)) y = yValue;
            }

            return new object[] { tipoMueble, x, y };
        }

        private object[] ParseColocarPuerta(string argumentsJson)
        {
            string tipoPuerta = "Single";
            double x = 0.0;
            double y = 0.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("tipoPuerta", out JsonElement tipoPuertaElement)) tipoPuerta = tipoPuertaElement.GetString() ?? tipoPuerta;
                if (root.TryGetProperty("x", out JsonElement xElement) && xElement.TryGetDouble(out double xValue)) x = xValue;
                if (root.TryGetProperty("y", out JsonElement yElement) && yElement.TryGetDouble(out double yValue)) y = yValue;
            }

            return new object[] { tipoPuerta, x, y };
        }

        private object[] ParseColocarVentana(string argumentsJson)
        {
            string tipoVentana = "Fixed";
            double x = 0.0;
            double y = 0.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("tipoVentana", out JsonElement tipoVentanaElement)) tipoVentana = tipoVentanaElement.GetString() ?? tipoVentana;
                if (root.TryGetProperty("x", out JsonElement xElement) && xElement.TryGetDouble(out double xValue)) x = xValue;
                if (root.TryGetProperty("y", out JsonElement yElement) && yElement.TryGetDouble(out double yValue)) y = yValue;
            }

            return new object[] { tipoVentana, x, y };
        }

        private object[] ParseCrearColumna(string argumentsJson)
        {
            string nivel = "Level 1";
            double x = 0.0;
            double y = 0.0;
            double altura = 3.0;
            string tipoColumna = string.Empty;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("nivel", out JsonElement nivelElement)) nivel = nivelElement.GetString() ?? nivel;
                if (root.TryGetProperty("x", out JsonElement xElement) && xElement.TryGetDouble(out double xValue)) x = xValue;
                if (root.TryGetProperty("y", out JsonElement yElement) && yElement.TryGetDouble(out double yValue)) y = yValue;
                if (root.TryGetProperty("altura", out JsonElement alturaElement) && alturaElement.TryGetDouble(out double alturaValue)) altura = alturaValue;
                if (root.TryGetProperty("tipoColumna", out JsonElement tipoColumnaElement)) tipoColumna = tipoColumnaElement.GetString() ?? string.Empty;
            }

            return new object[] { nivel, x, y, altura, tipoColumna };
        }

        private object[] ParseCrearViga(string argumentsJson)
        {
            string nivel = "Level 1";
            double x1 = 0.0;
            double y1 = 0.0;
            double x2 = 5.0;
            double y2 = 0.0;
            string tipoViga = string.Empty;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("nivel", out JsonElement nivelElement)) nivel = nivelElement.GetString() ?? nivel;
                if (root.TryGetProperty("x1", out JsonElement x1Element) && x1Element.TryGetDouble(out double x1Value)) x1 = x1Value;
                if (root.TryGetProperty("y1", out JsonElement y1Element) && y1Element.TryGetDouble(out double y1Value)) y1 = y1Value;
                if (root.TryGetProperty("x2", out JsonElement x2Element) && x2Element.TryGetDouble(out double x2Value)) x2 = x2Value;
                if (root.TryGetProperty("y2", out JsonElement y2Element) && y2Element.TryGetDouble(out double y2Value)) y2 = y2Value;
                if (root.TryGetProperty("tipoViga", out JsonElement tipoVigaElement)) tipoViga = tipoVigaElement.GetString() ?? string.Empty;
            }

            return new object[] { nivel, x1, y1, x2, y2, tipoViga };
        }

        private object[] ParseCrearTecho(string argumentsJson)
        {
            string nivel = "Level 1";
            string tipoTecho = string.Empty;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("nivel", out JsonElement nivelElement)) nivel = nivelElement.GetString() ?? nivel;
                if (root.TryGetProperty("tipoTecho", out JsonElement tipoTechoElement)) tipoTecho = tipoTechoElement.GetString() ?? string.Empty;
            }

            return new object[] { nivel, tipoTecho };
        }

        private async Task<string> ExecuteToolAsync(string methodName, object[] args)
        {
            if (_revitDispatcher == null)
            {
                return "Error: el dispatcher de Revit no está disponible en esta sesión.";
            }

            try
            {
                string resultado = string.Empty;
                await _revitTaskHandler.RunAsync(_ =>
                {
                    MethodInfo? method = _revitDispatcher.GetType().GetMethod(methodName);
                    if (method == null)
                    {
                        throw new MissingMethodException(_revitDispatcher.GetType().FullName, methodName);
                    }

                    object? invocationResult = method.Invoke(_revitDispatcher, args);
                    if (invocationResult is Task<string> task)
                    {
                        resultado = task.GetAwaiter().GetResult();
                    }
                    else if (invocationResult != null)
                    {
                        resultado = invocationResult.ToString() ?? string.Empty;
                    }
                });

                return string.IsNullOrWhiteSpace(resultado) ? "Completado" : resultado;
            }
            catch (TargetInvocationException ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
