using System.Text.Json.Serialization;

namespace Vigo360.VitrApi.Fetcher.Dto;

public class StoplistStop
{
    [JsonPropertyName("id")] public required int Id { get; set; }
    [JsonPropertyName("lineas")] public required string RawLines { get; set; }
    [JsonPropertyName("lat")] public required double Latitude { get; set; }
    [JsonPropertyName("lon")] public required double Longitude { get; set; }
    [JsonPropertyName("nombre")] public required string Name { get; set; }
    public List<string> Lineas => RawLines.Split(',').Select(s => s.Trim()).ToList();
}