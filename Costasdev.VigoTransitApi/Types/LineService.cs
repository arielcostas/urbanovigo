using System.Text.Json.Serialization;

namespace Costasdev.VigoTransitApi.Types;

public class LineService
{
    [JsonIgnore] public const string ImageBaseUri = "https://www.vitrasa.es/FotosFichas/lineas14/";

    public string Line { get; set; }
    public string ColourHex { get; set; }
    public LineServiceType Type { get; set; }
    public string SubType { get; set; }
    public string Description { get; set; }
    public Uri ImageUri { get; set; }

    public LineService(string line, string colourHex, LineServiceType type, string subType, string description, Uri imageUri)
    {
        Line = line;
        ColourHex = colourHex;
        Type = type;
        SubType = subType;
        Description = description;
        ImageUri = imageUri;
    }
}

public enum LineServiceType
{
    Airport,
    Auditorium,
    Circular,
    Citexvi,
    BusStation,
    Hospitals,
    MaritimeStation,
    Night,
    Beaches,
    Ptl,
    TrainStation,
    University,
    Unknown
}