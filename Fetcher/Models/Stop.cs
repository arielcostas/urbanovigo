using System.ComponentModel.DataAnnotations;

namespace Vigo360.VitrApi.Fetcher.Models;

[Serializable]
public class Stop(string id, string name, Arrival[] arrivals)
{
    [StringLength(6)] public string Id { get; init; } = id;
    [StringLength(50)] public string Name { get; init; } = name;
    public Arrival[] Arrivals { get; init; } = arrivals;
}