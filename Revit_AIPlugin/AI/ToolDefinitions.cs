using System;

namespace Revit_AIPlugin.AI
{
    // Definición simple de una "tool" que la IA puede invocar.
    public class ToolDefinition
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string JsonSchema { get; set; } = ""; // esquema JSON como string para referencia
    }

    public static class ToolDefinitions
    {
        public static readonly ToolDefinition CrearMuro = new()
        {
            Name = "CrearMuro",
            Description = "Crea un muro en el documento activo. Parámetros: altura (double), largo (double), material (string).",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"altura\":{\"type\":\"number\"},\"largo\":{\"type\":\"number\"},\"material\":{\"type\":\"string\"}},\"required\":[\"altura\",\"largo\"]}"
        };

        public static readonly ToolDefinition LeerElementos = new()
        {
            Name = "LeerElementos",
            Description = "Lee elementos del documento activo según un filtro simple. Parámetros: category (string, opcional).",
            JsonSchema = "{\"type\":\"object\",\"properties\":{\"category\":{\"type\":\"string\"}},\"required\":[]}"
        };
    }
}
