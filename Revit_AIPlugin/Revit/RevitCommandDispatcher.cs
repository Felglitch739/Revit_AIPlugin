using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using RevitAIPlugin.Revit.Tools;
using RevitAIPlugin.UI;

namespace Revit_AIPlugin
{
    public class RevitCommandDispatcher : IRevitCommandDispatcher
    {
        private readonly UIApplication _uiApp;
        private readonly CrearMuroHandler _crearMuroHandler;
        private readonly LeerElementosHandler _leerElementosHandler;
        private readonly CrearHabitacionEstructuradaHandler _crearHabitacionHandler;
        private readonly ColocarMobiliarioHandler _colocarMobiliarioHandler;
        private readonly ColocarPuertaHandler _colocarPuertaHandler;
        private readonly ColocarVentanaHandler _colocarVentanaHandler;
        private readonly ExternalEvent _crearMuroEvent;
        private readonly ExternalEvent _leerElementosEvent;
        private readonly ExternalEvent _crearHabitacionEvent;
        private readonly ExternalEvent _colocarMobiliarioEvent;
        private readonly ExternalEvent _colocarPuertaEvent;
        private readonly ExternalEvent _colocarVentanaEvent;

        public RevitCommandDispatcher(UIApplication uiApp)
        {
            _uiApp = uiApp;
            _crearMuroHandler = new CrearMuroHandler();
            _leerElementosHandler = new LeerElementosHandler();
            _crearHabitacionHandler = new CrearHabitacionEstructuradaHandler();
            _colocarMobiliarioHandler = new ColocarMobiliarioHandler();
            _colocarPuertaHandler = new ColocarPuertaHandler();
            _colocarVentanaHandler = new ColocarVentanaHandler();
            _crearMuroEvent = ExternalEvent.Create(_crearMuroHandler);
            _leerElementosEvent = ExternalEvent.Create(_leerElementosHandler);
            _crearHabitacionEvent = ExternalEvent.Create(_crearHabitacionHandler);
            _colocarMobiliarioEvent = ExternalEvent.Create(_colocarMobiliarioHandler);
            _colocarPuertaEvent = ExternalEvent.Create(_colocarPuertaHandler);
            _colocarVentanaEvent = ExternalEvent.Create(_colocarVentanaHandler);
        }

        public Task<string> CrearMuro(string nivel, double longitud, double altura, string tipoMuro)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _crearMuroHandler.TaskCompletionSource = tcs;
            _crearMuroHandler.Nivel = nivel;
            _crearMuroHandler.Longitud = longitud;
            _crearMuroHandler.Altura = altura;
            _crearMuroHandler.TipoMuro = tipoMuro;
            _crearMuroHandler.Resultado = null;

            try
            {
                _crearMuroEvent.Raise();
            }
            catch (Exception ex)
            {
                _crearMuroHandler.TaskCompletionSource = null;
                return Task.FromResult($"Error al disparar evento CrearMuro: {ex.Message}");
            }

            return tcs.Task;
        }

        public Task<string> LeerElementos(string categoria)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _leerElementosHandler.TaskCompletionSource = tcs;
            _leerElementosHandler.Categoria = categoria;
            _leerElementosHandler.Resultado = null;

            try
            {
                _leerElementosEvent.Raise();
            }
            catch (Exception ex)
            {
                _leerElementosHandler.TaskCompletionSource = null;
                return Task.FromResult($"Error al disparar evento LeerElementos: {ex.Message}");
            }

            return tcs.Task;
        }

        public Task<string> CrearHabitacionEstructurada(string nivel, double ancho, double largo, double altura)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _crearHabitacionHandler.TaskCompletionSource = tcs;
            _crearHabitacionHandler.Nivel = nivel;
            _crearHabitacionHandler.Ancho = ancho;
            _crearHabitacionHandler.Largo = largo;
            _crearHabitacionHandler.Altura = altura;
            _crearHabitacionHandler.Resultado = null;

            try
            {
                _crearHabitacionEvent.Raise();
            }
            catch (Exception ex)
            {
                _crearHabitacionHandler.TaskCompletionSource = null;
                return Task.FromResult($"Error al disparar evento CrearHabitacionEstructurada: {ex.Message}");
            }

            return tcs.Task;
        }

        public Task<string> ColocarMobiliario(string tipoMueble, double x, double y)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _colocarMobiliarioHandler.TaskCompletionSource = tcs;
            _colocarMobiliarioHandler.TipoMueble = tipoMueble;
            _colocarMobiliarioHandler.X = x;
            _colocarMobiliarioHandler.Y = y;
            _colocarMobiliarioHandler.Resultado = null;

            try
            {
                _colocarMobiliarioEvent.Raise();
            }
            catch (Exception ex)
            {
                _colocarMobiliarioHandler.TaskCompletionSource = null;
                return Task.FromResult($"Error al disparar evento ColocarMobiliario: {ex.Message}");
            }

            return tcs.Task;
        }

        public Task<string> ColocarPuerta(string tipoPuerta, double x, double y)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _colocarPuertaHandler.TaskCompletionSource = tcs;
            _colocarPuertaHandler.TipoPuerta = tipoPuerta;
            _colocarPuertaHandler.X = x;
            _colocarPuertaHandler.Y = y;
            _colocarPuertaHandler.Resultado = null;

            try
            {
                _colocarPuertaEvent.Raise();
            }
            catch (Exception ex)
            {
                _colocarPuertaHandler.TaskCompletionSource = null;
                return Task.FromResult($"Error al disparar evento ColocarPuerta: {ex.Message}");
            }

            return tcs.Task;
        }

        public Task<string> ColocarVentana(string tipoVentana, double x, double y)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _colocarVentanaHandler.TaskCompletionSource = tcs;
            _colocarVentanaHandler.TipoVentana = tipoVentana;
            _colocarVentanaHandler.X = x;
            _colocarVentanaHandler.Y = y;
            _colocarVentanaHandler.Resultado = null;

            try
            {
                _colocarVentanaEvent.Raise();
            }
            catch (Exception ex)
            {
                _colocarVentanaHandler.TaskCompletionSource = null;
                return Task.FromResult($"Error al disparar evento ColocarVentana: {ex.Message}");
            }

            return tcs.Task;
        }
    }
}
