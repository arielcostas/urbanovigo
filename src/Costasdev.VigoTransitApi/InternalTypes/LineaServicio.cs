using System.Text.Json.Serialization;

namespace Costasdev.VigoTransitApi.InternalTypes;

public class LineaServicio
{
    public string Linea { get; set; }
    public string? Color { get; set; }
    public string Tipo { get; set; }
    [JsonPropertyName("subtipo_es")] public string SubtipoEs { get; set; }
    [JsonPropertyName("subtipo_gl")] public string SubtipoGl { get; set; }
    public string Descripcion { get; set; }
    public string Imagen { get; set; }
}
