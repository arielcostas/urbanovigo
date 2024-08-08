using System.Text.Json.Serialization;

namespace Costasdev.VigoTransitApi.InternalTypes;

public class Cajero
{
    [JsonPropertyName("descri")] public string Descripcion { get; set; }
    [JsonPropertyName("codpos")] public int CodigoPostal { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("lon")] public decimal Longitude { get; set; }
    [JsonPropertyName("poblacion")] public string Poblacion { get; set; }
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("provincia")] public string Provincia { get; set; }
    [JsonPropertyName("lat")] public decimal Latitude { get; set; }
}