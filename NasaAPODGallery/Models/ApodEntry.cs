namespace NasaAPODGallery.Models;

public class ApodEntry
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string MediaType { get; set; } = string.Empty;

    public string? ServiceVersion { get; set; }

    public DateTime SavedAt { get; set; }

    public int? PostId { get; set; }

    public string? Permalink { get; set; }

    public string? HdUrl { get; set; }

    public string? Credit { get; set; }

    public string? Copyright { get; set; }

    public string? Alt { get; set; }
}