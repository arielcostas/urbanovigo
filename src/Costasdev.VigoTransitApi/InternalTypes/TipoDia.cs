using System.Text.Json.Serialization;

namespace Costasdev.VigoTransitApi.InternalTypes;

public class TipoDia
{
    [JsonPropertyName("tipo_dia")] public string Tipo { get; set; }
}