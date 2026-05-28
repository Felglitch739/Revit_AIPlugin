using Autodesk.Revit.UI;
using RevitAIPlugin.Revit;

namespace Revit_AIPlugin
{
    public class App : IExternalApplication
    {
        // Singleton para acceder desde cualquier parte del plugin
        public static App Instance { get; private set; }
        public UIApplication UIApp { get; private set; }
        public RevitCommandDispatcher Dispatcher { get; private set; }

        public Result OnStartup(UIControlledApplication application)
        {
            Instance = this;

            // Crear el Tab principal en el Ribbon de Revit
            string tabName = "IA Plugin";
            application.CreateRibbonTab(tabName);

            // Crear el Panel dentro del Tab
            RibbonPanel panel = application.CreateRibbonPanel(tabName, "Asistente IA");

            // Ruta del DLL compilado
            string assemblyPath = typeof(App).Assembly.Location;

            // Botón principal para abrir el chat
            PushButtonData buttonData = new PushButtonData(
                "btnAbrirChat",
                "Abrir Chat IA",
                assemblyPath,
                "Revit_AIPlugin.CommandAbrirChat"
            );

            PushButton button = panel.AddItem(buttonData) as PushButton;
            button.ToolTip = "Abre el asistente de IA para controlar Revit con lenguaje natural";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            Instance = null;
            UIApp = null;
            Dispatcher = null;
            return Result.Succeeded;
        }

        // Llamado desde CommandAbrirChat para inicializar el Dispatcher con UIApplication
        public void Initialize(UIApplication uiApp)
        {
            if (UIApp == null)
            {
                UIApp = uiApp;
                Dispatcher = new RevitCommandDispatcher(uiApp);
            }
        }
    }
}
