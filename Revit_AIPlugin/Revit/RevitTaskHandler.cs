using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace Revit_AIPlugin
{
    public class RevitTaskHandler : IExternalEventHandler
    {
        private readonly ExternalEvent _externalEvent;
        private Action<UIApplication> _currentAction;
        private TaskCompletionSource<bool> _tcs;
        private readonly object _lock = new object();

        public RevitTaskHandler()
        {
            _externalEvent = ExternalEvent.Create(this);
        }

        public Task RunAsync(Action<UIApplication> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            lock (_lock)
            {
                if (_tcs != null && !_tcs.Task.IsCompleted)
                {
                    throw new InvalidOperationException("Ya existe una operación de Revit en curso.");
                }

                _tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                _currentAction = action;

                try
                {
                    _externalEvent.Raise();
                }
                catch (Exception ex)
                {
                    _tcs.TrySetException(ex);
                    _currentAction = null;
                }

                return _tcs.Task;
            }
        }

        public void Execute(UIApplication app)
        {
            try
            {
                _currentAction?.Invoke(app);
                _tcs?.TrySetResult(true);
            }
            catch (Exception ex)
            {
                _tcs?.TrySetException(ex);
            }
            finally
            {
                _currentAction = null;
                _tcs = null;
            }
        }

        public string GetName() => nameof(RevitTaskHandler);
    }
}
