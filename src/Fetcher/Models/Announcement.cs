using System.ComponentModel.DataAnnotations;

namespace Vigo360.VitrApi.Fetcher.Models;

public class Announcement
{
    public int Id { get; init; }
    public DateOnly PublicationDate { get; init; }
    [StringLength(128)] public string Title { get; init; } = string.Empty;
    [StringLength(5000)] public string Body { get; init; } = string.Empty;
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}