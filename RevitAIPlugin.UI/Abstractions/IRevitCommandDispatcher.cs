using System.Threading.Tasks;

namespace RevitAIPlugin.UI
{
    public interface IRevitCommandDispatcher
    {
        Task<string> CrearMuro(string nivel, double longitud, double altura, string tipoMuro);
        Task<string> LeerElementos(string categoria);
        Task<string> CrearHabitacionEstructurada(string nivel, double ancho, double largo, double altura);
        Task<string> ColocarMobiliario(string tipoMueble, double x, double y);
        Task<string> ColocarPuerta(string tipoPuerta, double x, double y);
        Task<string> ColocarVentana(string tipoVentana, double x, double y);
    }
}
