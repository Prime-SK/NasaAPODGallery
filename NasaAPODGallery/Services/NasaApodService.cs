using NasaAPODGallery.DTOs;
using System.Net.Http.Json;

namespace NasaAPODGallery.Services;

public class NasaApodService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public NasaApodService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["Nasa:ApiKey"]
            ?? throw new InvalidOperationException(
                "NASA API key is missing.");
    }

    public async Task<List<ApodDto>> GetApodsAsync(
        DateOnly startDate,
        DateOnly endDate)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException(
                "Start date must be on or before end date.");
        }

        string url =
            "https://science.nasa.gov/wp-json/wp/v2/apod-basic/" +
            $"?api_key={Uri.EscapeDataString(_apiKey)}" +
            $"&date_from={startDate:yyMMdd}" +
            $"&date_to={endDate:yyMMdd}";

        using var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var entries = await response.Content
            .ReadFromJsonAsync<List<ApodDto>>();

        return entries ?? new List<ApodDto>();
    }
}