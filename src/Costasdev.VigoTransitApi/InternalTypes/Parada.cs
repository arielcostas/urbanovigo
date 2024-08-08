using System.Text.Json.Serialization;

namespace Costasdev.VigoTransitApi.InternalTypes;

public class Parada
{
    [JsonPropertyName("lon")] public decimal Lon { get; set; }
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("nombre")] public string Nombre { get; set; }
    [JsonPropertyName("lat")] public decimal Lat { get; set; }
    [JsonPropertyName("stop_id")] public string StopId { get; set; }
    [JsonPropertyName("lineas")] public string? Lineas { get; set; }
}