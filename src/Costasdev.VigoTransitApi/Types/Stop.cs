namespace Costasdev.VigoTransitApi.Types;

public class Stop
{
    public int StopId { get; set; }
    public string InternalStopId { get; set; }
    public string Name { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public List<string>? Lines { get; set; }
}