using System;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using RevitAIPlugin.Revit.Tools;

namespace Revit_AIPlugin
{
    public class RevitCommandDispatcher
    {
        private readonly UIApplication _uiApp;

        // ── Handlers ──────────────────────────────────────────────────────────
        private readonly CrearMuroHandler                    _crearMuroHandler;
        private readonly LeerElementosHandler                _leerElementosHandler;
        private readonly CrearHabitacionEstructuradaHandler  _crearHabitacionHandler;
        private readonly ColocarMobiliarioHandler            _colocarMobiliarioHandler;
        private readonly ColocarPuertaHandler                _colocarPuertaHandler;
        private readonly ColocarVentanaHandler               _colocarVentanaHandler;

        // ── ExternalEvents ────────────────────────────────────────────────────
        private readonly ExternalEvent _crearMuroEvent;
        private readonly ExternalEvent _leerElementosEvent;
        private readonly ExternalEvent _crearHabitacionEvent;
        private readonly ExternalEvent _colocarMobiliarioEvent;
        private readonly ExternalEvent _colocarPuertaEvent;
        private readonly ExternalEvent _colocarVentanaEvent;

        // Timeout global para que la UI nunca se quede colgada
        private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(45);

        public RevitCommandDispatcher(UIApplication uiApp)
        {
            _uiApp = uiApp;

            _crearMuroHandler            = new CrearMuroHandler();
            _leerElementosHandler        = new LeerElementosHandler();
            _crearHabitacionHandler      = new CrearHabitacionEstructuradaHandler();
            _colocarMobiliarioHandler    = new ColocarMobiliarioHandler();
            _colocarPuertaHandler        = new ColocarPuertaHandler();
            _colocarVentanaHandler       = new ColocarVentanaHandler();

            _crearMuroEvent              = ExternalEvent.Create(_crearMuroHandler);
            _leerElementosEvent          = ExternalEvent.Create(_leerElementosHandler);
            _crearHabitacionEvent        = ExternalEvent.Create(_crearHabitacionHandler);
            _colocarMobiliarioEvent      = ExternalEvent.Create(_colocarMobiliarioHandler);
            _colocarPuertaEvent          = ExternalEvent.Create(_colocarPuertaHandler);
            _colocarVentanaEvent         = ExternalEvent.Create(_colocarVentanaHandler);
        }

        // =====================================================================
        //  MÉTODO CENTRAL — toda la lógica del puente está aquí.
        //  Nunca más copiar este patrón a mano en cada método.
        // =====================================================================
        private async Task<string> Despachar(
            Action preparar,
            IHandlerConTCS handler,
            ExternalEvent evento)
        {
            // RunContinuationsAsynchronously evita el deadlock cuando el Set()
            // y el await ocurren en el mismo SynchronizationContext de WPF.
            var tcs = new TaskCompletionSource<string>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            // 1. Cargar parámetros en el handler (ya los puso el llamador)
            preparar();

            // 2. Asignar el TCS ANTES de Raise() — la carrera es posible
            handler.TaskCompletionSource = tcs;

            // 3. Levantar el evento; Revit lo ejecutará en su hilo principal
            try
            {
                var status = evento.Raise();
                if (status != ExternalEventRequest.Accepted)
                {
                    handler.TaskCompletionSource = null;
                    return $"ERROR: Revit rechazó el evento ({status}). " +
                           "Asegúrate de que no haya otra transacción activa.";
                }
            }
            catch (Exception ex)
            {
                handler.TaskCompletionSource = null;
                return $"ERROR al disparar evento: {ex.Message}";
            }

            // 4. Timeout de seguridad: si Revit nunca llama Execute(), liberamos igual
            using var cts = new CancellationTokenSource(TIMEOUT);
            cts.Token.Register(() =>
                tcs.TrySetResult(
                    "ERROR: Timeout — Revit no ejecutó el handler en 45 s. " +
                    "Comprueba que Revit esté en primer plano y sin diálogos abiertos."));

            return await tcs.Task.ConfigureAwait(false);
        }

        // =====================================================================
        //  API PÚBLICA — cada método solo prepara parámetros y llama Despachar
        // =====================================================================

        public Task<string> CrearMuro(
            string nivel, double longitud, double altura, string tipoMuro)
        {
            return Despachar(
                preparar: () =>
                {
                    _crearMuroHandler.Nivel     = nivel;
                    _crearMuroHandler.Longitud  = longitud;
                    _crearMuroHandler.Altura    = altura;
                    _crearMuroHandler.TipoMuro  = tipoMuro;
                    _crearMuroHandler.Resultado = null;
                },
                handler: _crearMuroHandler,
                evento:  _crearMuroEvent);
        }

        public Task<string> LeerElementos(string categoria)
        {
            return Despachar(
                preparar: () =>
                {
                    _leerElementosHandler.Categoria = categoria;
                    _leerElementosHandler.Resultado = null;
                },
                handler: _leerElementosHandler,
                evento:  _leerElementosEvent);
        }

        public Task<string> CrearHabitacionEstructurada(
            string nivel, double ancho, double largo, double altura)
        {
            return Despachar(
                preparar: () =>
                {
                    _crearHabitacionHandler.Nivel    = nivel;
                    _crearHabitacionHandler.Ancho    = ancho;
                    _crearHabitacionHandler.Largo    = largo;
                    _crearHabitacionHandler.Altura   = altura;
                    _crearHabitacionHandler.Resultado = null;
                },
                handler: _crearHabitacionHandler,
                evento:  _crearHabitacionEvent);
        }

        public Task<string> ColocarMobiliario(
            string tipoMueble, double x, double y)
        {
            return Despachar(
                preparar: () =>
                {
                    _colocarMobiliarioHandler.TipoMueble = tipoMueble;
                    _colocarMobiliarioHandler.X          = x;
                    _colocarMobiliarioHandler.Y          = y;
                    _colocarMobiliarioHandler.Resultado  = null;
                },
                handler: _colocarMobiliarioHandler,
                evento:  _colocarMobiliarioEvent);
        }

        public Task<string> ColocarPuerta(
            string tipoPuerta, double x, double y)
        {
            return Despachar(
                preparar: () =>
                {
                    _colocarPuertaHandler.TipoPuerta = tipoPuerta;
                    _colocarPuertaHandler.X          = x;
                    _colocarPuertaHandler.Y          = y;
                    _colocarPuertaHandler.Resultado  = null;
                },
                handler: _colocarPuertaHandler,
                evento:  _colocarPuertaEvent);
        }

        public Task<string> ColocarVentana(
            string tipoVentana, double x, double y)
        {
            return Despachar(
                preparar: () =>
                {
                    _colocarVentanaHandler.TipoVentana = tipoVentana;
                    _colocarVentanaHandler.X           = x;
                    _colocarVentanaHandler.Y           = y;
                    _colocarVentanaHandler.Resultado   = null;
                },
                handler: _colocarVentanaHandler,
                evento:  _colocarVentanaEvent);
        }
    }
}
