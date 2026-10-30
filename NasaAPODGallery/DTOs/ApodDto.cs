using System.Text.Json.Serialization;

namespace NasaAPODGallery.DTOs;

public class ApodDto
{
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("media_type")]
    public string MediaType { get; set; } = string.Empty;

    [JsonPropertyName("hdurl")]
    public string? HdUrl { get; set; }

    [JsonPropertyName("service_version")]
    public string? ServiceVersion { get; set; }

    [JsonPropertyName("post_id")]
    public int? PostId { get; set; }

    [JsonPropertyName("permalink")]
    public string? Permalink { get; set; }

    [JsonPropertyName("credit")]
    public string? Credit { get; set; }

    [JsonPropertyName("copyright")]
    public string? Copyright { get; set; }

    [JsonPropertyName("alt")]
    public string? Alt { get; set; }
}