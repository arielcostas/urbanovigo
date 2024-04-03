using System.ComponentModel.DataAnnotations;

namespace Vigo360.VitrApi.Fetcher.Models;

[Serializable]
public class Arrival(string line, string headsign, string estimatedMinutes)
{
    [StringLength(6)] public string Line { get; init; } = line;
    [StringLength(50)] public string Headsign { get; init; } = headsign;
    [StringLength(3)] public string EstimatedMinutes { get; init; } = estimatedMinutes;
}