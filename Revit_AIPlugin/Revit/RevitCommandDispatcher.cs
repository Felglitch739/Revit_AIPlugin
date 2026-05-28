using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using RevitAIPlugin.Revit.Tools;

namespace RevitAIPlugin.Revit
{
    /// <summary>
    /// Puente entre el hilo async de la IA y el hilo principal de Revit.
    /// Usa ExternalEvent para ejecutar código de Revit API de forma segura.
    /// </summary>
    public class RevitCommandDispatcher
    {
        private readonly UIApplication _uiApp;
        private readonly CrearMuroHandler _crearMuroHandler;
        private readonly LeerElementosHandler _leerElementosHandler;

        private readonly ExternalEvent _crearMuroEvent;
        private readonly ExternalEvent _leerElementosEvent; 

        public RevitCommandDispatcher(UIApplication uiApp)
        {
            _uiApp = uiApp;

            _crearMuroHandler = new CrearMuroHandler();
            _leerElementosHandler = new LeerElementosHandler();

            _crearMuroEvent = ExternalEvent.Create(_crearMuroHandler);
            _leerElementosEvent = ExternalEvent.Create(_leerElementosHandler);
        }

        /// <summary>
        /// Solicita crear un muro y espera el resultado de forma async.
        /// </summary>
        public async Task<string> CrearMuro(string nivel, double longitud, double altura, string tipoMuro)
        {
            _crearMuroHandler.Nivel = nivel;
            _crearMuroHandler.Longitud = longitud;
            _crearMuroHandler.Altura = altura;
            _crearMuroHandler.TipoMuro = tipoMuro;
            _crearMuroHandler.Resultado = null;

            _crearMuroEvent.Raise();

            // Esperar hasta que Revit procese el evento (máx. 10 segundos)
            return await EsperarResultado(() => _crearMuroHandler.Resultado);
        }

        /// <summary>
        /// Solicita leer elementos del modelo y espera el resultado.
        /// </summary>
        public async Task<string> LeerElementos(string categoria)
        {
            _leerElementosHandler.Categoria = categoria;
            _leerElementosHandler.Resultado = null;

            _leerElementosEvent.Raise();

            return await EsperarResultado(() => _leerElementosHandler.Resultado);
        }

        /// <summary>
        /// Polling async que espera a que un handler de Revit complete su tarea.
        /// </summary>
        private async Task<string> EsperarResultado(Func<string> obtenerResultado, int timeoutMs = 10000)
        {
            int elapsed = 0;
            const int intervalo = 100;

            while (elapsed < timeoutMs)
            {
                string resultado = obtenerResultado();
                if (resultado != null)
                    return resultado;

                await Task.Delay(intervalo);
                elapsed += intervalo;
            }

            return "Error: Timeout al esperar respuesta de Revit.";
        }
    }
}
