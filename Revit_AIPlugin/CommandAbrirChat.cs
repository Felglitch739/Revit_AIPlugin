using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAIPlugin.UI;

namespace Revit_AIPlugin
{
    [Transaction(TransactionMode.Manual)]
    public class CommandAbrirChat : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Inicializar el Dispatcher con acceso al hilo de Revit
            App.Instance.Initialize(commandData.Application);

            var ventana = new RevitAIPlugin.UI.MainWindow();
            ventana.Topmost = true;
            ventana.Show();
            return Result.Succeeded;
        }
    }
}
