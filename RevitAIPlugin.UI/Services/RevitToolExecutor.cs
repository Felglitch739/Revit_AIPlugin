using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace RevitAIPlugin.UI
{
    internal sealed class RevitToolExecutor : IToolExecutor
    {
        private readonly IRevitCommandDispatcher? _revitDispatcher;

        public RevitToolExecutor(IRevitCommandDispatcher? revitDispatcher)
        {
            _revitDispatcher = revitDispatcher;
        }

        public async Task<string> ExecuteAsync(GroqToolCall toolCall)
        {
            if (_revitDispatcher == null)
            {
                return "Error: el dispatcher de Revit no está disponible en esta sesión.";
            }

            // Encolar la ejecución para que la UI no lance timeout prematuros y Revit procese secuencialmente
            return toolCall.Name switch
            {
                "LeerElementos" => await ExecuteLeerElementosAsync(toolCall.ArgumentsJson),
                "CrearMuro" => await ExecuteCrearMuroAsync(toolCall.ArgumentsJson),
                "CrearHabitacionEstructurada" => await ExecuteCrearHabitacionEstructuradaAsync(toolCall.ArgumentsJson),
                "ColocarMobiliario" => await ExecuteColocarMobiliarioAsync(toolCall.ArgumentsJson),
                "ColocarPuerta" => await ExecuteColocarPuertaAsync(toolCall.ArgumentsJson),
                "ColocarVentana" => await ExecuteColocarVentanaAsync(toolCall.ArgumentsJson),
                _ => $"Error: herramienta no soportada '{toolCall.Name}'."
            };
        }

        private async Task<string> ExecuteLeerElementosAsync(string argumentsJson)
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

            return await _revitDispatcher!.LeerElementos(categoria);
        }

        private async Task<string> ExecuteCrearMuroAsync(string argumentsJson)
        {
            string nivel = "Level 1";
            string tipoMuro = string.Empty;
            double longitud = 5.0;
            double altura = 3.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("nivel", out JsonElement nivelElement))
                {
                    nivel = nivelElement.GetString() ?? nivel;
                }

                if (root.TryGetProperty("tipoMuro", out JsonElement tipoMuroElement))
                {
                    tipoMuro = tipoMuroElement.GetString() ?? string.Empty;
                }

                if (root.TryGetProperty("longitud", out JsonElement longitudElement) && longitudElement.TryGetDouble(out double longitudValue))
                {
                    longitud = longitudValue;
                }

                if (root.TryGetProperty("altura", out JsonElement alturaElement) && alturaElement.TryGetDouble(out double alturaValue))
                {
                    altura = alturaValue;
                }
            }

            return await _revitDispatcher!.CrearMuro(nivel, longitud, altura, tipoMuro);
        }

        private async Task<string> ExecuteCrearHabitacionEstructuradaAsync(string argumentsJson)
        {
            string nivel = "Level 1";
            double ancho = 4.0;
            double largo = 5.0;
            double altura = 3.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("nivel", out JsonElement nivelElement))
                {
                    nivel = nivelElement.GetString() ?? nivel;
                }

                if (root.TryGetProperty("ancho", out JsonElement anchoElement) && anchoElement.TryGetDouble(out double anchoValue))
                {
                    ancho = anchoValue;
                }

                if (root.TryGetProperty("largo", out JsonElement largoElement) && largoElement.TryGetDouble(out double largoValue))
                {
                    largo = largoValue;
                }

                if (root.TryGetProperty("altura", out JsonElement alturaElement) && alturaElement.TryGetDouble(out double alturaValue))
                {
                    altura = alturaValue;
                }
            }

            return await _revitDispatcher!.CrearHabitacionEstructurada(nivel, ancho, largo, altura);
        }

        private async Task<string> ExecuteColocarMobiliarioAsync(string argumentsJson)
        {
            string tipoMueble = "";
            double x = 0.0;
            double y = 0.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("tipoMueble", out JsonElement tipoMuebleElement))
                {
                    tipoMueble = tipoMuebleElement.GetString() ?? string.Empty;
                }

                if (root.TryGetProperty("x", out JsonElement xElement) && xElement.TryGetDouble(out double xValue))
                {
                    x = xValue;
                }

                if (root.TryGetProperty("y", out JsonElement yElement) && yElement.TryGetDouble(out double yValue))
                {
                    y = yValue;
                }
            }

            return await _revitDispatcher!.ColocarMobiliario(tipoMueble, x, y);
        }

        private async Task<string> ExecuteColocarPuertaAsync(string argumentsJson)
        {
            string tipoPuerta = "Single";
            double x = 0.0;
            double y = 0.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("tipoPuerta", out JsonElement tipoPuertaElement))
                {
                    tipoPuerta = tipoPuertaElement.GetString() ?? tipoPuerta;
                }

                if (root.TryGetProperty("x", out JsonElement xElement) && xElement.TryGetDouble(out double xValue))
                {
                    x = xValue;
                }

                if (root.TryGetProperty("y", out JsonElement yElement) && yElement.TryGetDouble(out double yValue))
                {
                    y = yValue;
                }
            }

            return await _revitDispatcher!.ColocarPuerta(tipoPuerta, x, y);
        }

        private async Task<string> ExecuteColocarVentanaAsync(string argumentsJson)
        {
            string tipoVentana = "Fixed";
            double x = 0.0;
            double y = 0.0;

            if (!string.IsNullOrWhiteSpace(argumentsJson))
            {
                using JsonDocument doc = JsonDocument.Parse(argumentsJson);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("tipoVentana", out JsonElement tipoVentanaElement))
                {
                    tipoVentana = tipoVentanaElement.GetString() ?? tipoVentana;
                }

                if (root.TryGetProperty("x", out JsonElement xElement) && xElement.TryGetDouble(out double xValue))
                {
                    x = xValue;
                }

                if (root.TryGetProperty("y", out JsonElement yElement) && yElement.TryGetDouble(out double yValue))
                {
                    y = yValue;
                }
            }

            return await _revitDispatcher!.ColocarVentana(tipoVentana, x, y);
        }
    }
}
