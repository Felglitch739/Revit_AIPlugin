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
            Description = "Coloca mobiliario en una coordenada X e Y específica. Para camas, usa exactamente 'bed-standard', 'bed-shaker' o 'bed-box' si están cargadas en el proyecto.",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"tipoMueble\":{\"type\":\"string\",\"description\":\"Nombre exacto de la familia cargada en el proyecto (por ejemplo: 'bed-standard', 'bed-shaker', 'bed-box')\"},\"x\":{\"type\":\"number\",\"description\":\"Coordenada X en metros relativa al centro de la habitación\"},\"y\":{\"type\":\"number\",\"description\":\"Coordenada Y en metros relativa al centro de la habitación\"}},\"required\":[\"tipoMueble\",\"x\",\"y\"]}"
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

        public static readonly ToolDefinition CrearColumna = new()
        {
            Name = "CrearColumna",
            Description = "Crea una columna estructural en una coordenada X e Y específica en el modelo. Parámetros: nivel (string opcional), x (double en metros), y (double en metros), altura (double en metros), tipoColumna (string opcional).",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"nivel\":{\"type\":\"string\",\"description\":\"Nombre del nivel, ej. 'Level 1'\"},\"x\":{\"type\":\"number\",\"description\":\"Coordenada X en metros\"},\"y\":{\"type\":\"number\",\"description\":\"Coordenada Y en metros\"},\"altura\":{\"type\":\"number\",\"description\":\"Altura de la columna en metros\"},\"tipoColumna\":{\"type\":\"string\",\"description\":\"Tipo o nombre de la columna\"}},\"required\":[\"x\",\"y\"]}"
        };

        public static readonly ToolDefinition CrearViga = new()
        {
            Name = "CrearViga",
            Description = "Crea una viga estructural entre dos puntos. Parámetros: nivel (string opcional), x1/y1 (punto inicio), x2/y2 (punto fin), tipoViga (string opcional).",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"nivel\":{\"type\":\"string\",\"description\":\"Nombre del nivel, ej. 'Level 1'\"},\"x1\":{\"type\":\"number\",\"description\":\"Coordenada X inicial en metros\"},\"y1\":{\"type\":\"number\",\"description\":\"Coordenada Y inicial en metros\"},\"x2\":{\"type\":\"number\",\"description\":\"Coordenada X final en metros\"},\"y2\":{\"type\":\"number\",\"description\":\"Coordenada Y final en metros\"},\"tipoViga\":{\"type\":\"string\",\"description\":\"Tipo o nombre de la viga\"}},\"required\":[\"x1\",\"y1\",\"x2\",\"y2\"]}"
        };

        public static readonly ToolDefinition CrearTecho = new()
        {
            Name = "CrearTecho",
            Description = "Crea un techo en el modelo basado en los muros del nivel especificado. Parámetros: nivel (string), tipoTecho (string opcional).",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"nivel\":{\"type\":\"string\",\"description\":\"Nombre del nivel, ej. 'Level 1'\"},\"tipoTecho\":{\"type\":\"string\",\"description\":\"Tipo o nombre del techo\"}},\"required\":[\"nivel\"]}"
        };
    }
}
