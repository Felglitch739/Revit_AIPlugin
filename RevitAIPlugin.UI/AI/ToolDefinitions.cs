namespace RevitAIPlugin.UI
{
    public sealed class ToolDefinition
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string JsonSchema { get; init; } = string.Empty;
    }

    public static class ToolDefinitions
    {
        public static readonly ToolDefinition CrearMuro = new()
        {
            Name = "CrearMuro",
            Description = "Crea un muro en Revit. Parámetros: nivel (string opcional), longitud (double en metros), altura (double en metros), tipoMuro (string opcional).",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"nivel\":{\"type\":\"string\"},\"longitud\":{\"type\":\"number\"},\"altura\":{\"type\":\"number\"},\"tipoMuro\":{\"type\":\"string\"}},\"required\":[\"longitud\",\"altura\"]}"
        };

        public static readonly ToolDefinition LeerElementos = new()
        {
            Name = "LeerElementos",
            Description = "Lee y cuenta elementos de una categoría del modelo. Parámetro: categoria (string opcional).",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"categoria\":{\"type\":\"string\"}},\"required\":[]}"
        };

        public static readonly ToolDefinition CrearHabitacionEstructurada = new()
        {
            Name = "CrearHabitacionEstructurada",
            Description = "Crea los muros perimetrales de una habitación rectangular en Revit a partir de ancho, largo y altura en metros.",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"ancho\":{\"type\":\"number\",\"description\":\"Ancho de la habitación en metros\"},\"largo\":{\"type\":\"number\",\"description\":\"Largo de la habitación en metros\"},\"altura\":{\"type\":\"number\",\"description\":\"Altura de los muros en metros\"},\"nivel\":{\"type\":\"string\",\"description\":\"Nombre del nivel, por ejemplo 'Level 1' o 'Nivel 1'\"}},\"required\":[\"ancho\",\"largo\",\"nivel\"]}"
        };

        public static readonly ToolDefinition ColocarMobiliario = new()
        {
            Name = "ColocarMobiliario",
            Description = "Coloca un elemento de mobiliario (cama, escritorio, silla) en una coordenada X e Y específica dentro de la habitación.",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"tipoMueble\":{\"type\":\"string\",\"description\":\"Tipo o nombre del mueble (ej. 'Cama', 'Silla', 'Escritorio')\"},\"x\":{\"type\":\"number\",\"description\":\"Coordenada X en metros relativa al centro de la habitación\"},\"y\":{\"type\":\"number\",\"description\":\"Coordenada Y en metros relativa al centro de la habitación\"}},\"required\":[\"tipoMueble\",\"x\",\"y\"]}"
        };

        public static readonly ToolDefinition ColocarPuerta = new()
        {
            Name = "ColocarPuerta",
            Description = "Coloca una puerta en una coordenada X e Y sobre los muros perimetrales. Las coordenadas deben caer exactamente sobre los ejes perimetrales (ej. X=ancho/2, Y=0 para pared inferior).",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"tipoPuerta\":{\"type\":\"string\",\"description\":\"Tipo o nombre de la puerta (ej. 'Single', 'Double', 'Sliding')\"},\"x\":{\"type\":\"number\",\"description\":\"Coordenada X en metros sobre el muro\"},\"y\":{\"type\":\"number\",\"description\":\"Coordenada Y en metros sobre el muro\"}},\"required\":[\"x\",\"y\"]}"
        };

        public static readonly ToolDefinition ColocarVentana = new()
        {
            Name = "ColocarVentana",
            Description = "Coloca una ventana en una coordenada X e Y sobre los muros perimetrales. Las coordenadas deben caer exactamente sobre los ejes perimetrales (ej. X=ancho/2, Y=0 para pared inferior).",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"tipoVentana\":{\"type\":\"string\",\"description\":\"Tipo o nombre de la ventana (ej. 'Fixed', 'Operable', 'Glass')\"},\"x\":{\"type\":\"number\",\"description\":\"Coordenada X en metros sobre el muro\"},\"y\":{\"type\":\"number\",\"description\":\"Coordenada Y en metros sobre el muro\"}},\"required\":[\"x\",\"y\"]}"
        };
    }
}
