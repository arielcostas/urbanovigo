using System.Text.Json.Serialization;

namespace Costasdev.VigoTransitApi.InternalTypes;

public class EstimacionesParadaResponse
{
    [JsonPropertyName("parada")] public List<InfoParada> Parada { get; set; }
    [JsonPropertyName("estimaciones")] public List<EstimacionesParada> Estimaciones { get; set; }

    public class InfoParada
    {
        [JsonPropertyName("nombre")] public string Nombre { get; set; }
        [JsonPropertyName("stop_vitrasa")] public int Id { get; set; }
        [JsonPropertyName("latitud")] public decimal Latitud { get; set; }
        [JsonPropertyName("longitud")] public decimal Longitud { get; set; }
    }
}

public class EstimacionesParada
{
    [JsonPropertyName("linea")] public string Linea { get; set; }
    [JsonPropertyName("ruta")] public string Ruta { get; set; }
    [JsonPropertyName("minutos")] public int Minutos { get; set; }
    [JsonPropertyName("metros")] public int Metros { get; set; }

    public EstimacionesParada(string linea, string ruta, int minutos, int metros)
    {
        Linea = linea;
        Ruta = ruta;
        Minutos = minutos;
        Metros = metros;
    }
}