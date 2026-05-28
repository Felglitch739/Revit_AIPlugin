using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text.Json;

namespace Revit_AIPlugin.Revit.Tools
{
    public static class CrearMuroTool
    {
        public static string Run(UIApplication uiapp, string jsonArgs)
        {
            try
            {
                using var doc = uiapp?.ActiveUIDocument?.Document;
                if (doc == null) return "Documento no disponible";

                var args = JsonSerializer.Deserialize<CrearMuroArgs>(jsonArgs);
                if (args == null) return "Argumentos inválidos";

                // Aquí pondríamos la lógica para crear el muro con Revit API
                // Para mantener el ejemplo simple, devolvemos una cadena de éxito.

                return $"Muro creado (simulado): altura={args.altura}, largo={args.largo}, material={args.material}";
            }
            catch (Exception ex)
            {
                return $"Error CrearMuroTool: {ex.Message}";
            }
        }

        private class CrearMuroArgs
        {
            public double altura { get; set; }
            public double largo { get; set; }
            public string material { get; set; } = "Concreto";
        }
    }
}
