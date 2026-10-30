using Microsoft.Data.SqlClient;
using NasaAPODGallery.Models;
using System.Data;

namespace NasaAPODGallery.Repositories;

public class ApodRepository
{
    private readonly string _connectionString;

    public ApodRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Database connection string is missing.");
    }

    public async Task<List<ApodEntry>> GetAllAsync()
    {
        var entries = new List<ApodEntry>();

        using var connection = new SqlConnection(_connectionString);

        const string sql = """
            SELECT Id, [Date], Title, Explanation, Url,
                   MediaType, ServiceVersion, SavedAt,
                   PostId, Permalink, HdUrl, Credit, Copyright, Alt
            FROM dbo.ApodEntries
            ORDER BY [Date] DESC;
            """;

        using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            entries.Add(new ApodEntry
            {
                Id = reader.GetInt32(0),
                Date = DateOnly.FromDateTime(reader.GetDateTime(1)),
                Title = reader.GetString(2),
                Explanation = reader.GetString(3),
                Url = reader.GetString(4),
                MediaType = reader.GetString(5),
                ServiceVersion = reader.IsDBNull(6)
                    ? null
                    : reader.GetString(6),
                SavedAt = reader.GetDateTime(7),
                PostId = reader.IsDBNull(8) ? null : reader.GetInt32(8),
                Permalink = reader.IsDBNull(9) ? null : reader.GetString(9),
                HdUrl = reader.IsDBNull(10) ? null : reader.GetString(10),
                Credit = reader.IsDBNull(11) ? null : reader.GetString(11),
                Copyright = reader.IsDBNull(12) ? null : reader.GetString(12),
                Alt = reader.IsDBNull(13) ? null : reader.GetString(13)
            });
        }

        return entries;
    }

    public async Task<bool> SaveAsync(ApodEntry entry)
    {
        using var connection = new SqlConnection(_connectionString);

        const string sql = """
            INSERT INTO dbo.ApodEntries
                ([Date], Title, Explanation, Url, MediaType, ServiceVersion,
                 PostId, Permalink, HdUrl, Credit, Copyright, Alt)
            SELECT
                @Date, @Title, @Explanation, @Url, @MediaType, @ServiceVersion,
                @PostId, @Permalink, @HdUrl, @Credit, @Copyright, @Alt
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM dbo.ApodEntries WITH (UPDLOCK, HOLDLOCK)
                WHERE [Date] = @Date
            );
            """;

        using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@Date", SqlDbType.Date).Value =
            entry.Date.ToDateTime(TimeOnly.MinValue);

        command.Parameters.Add("@Title", SqlDbType.NVarChar, 500).Value =
            entry.Title;

        command.Parameters.Add("@Explanation", SqlDbType.NVarChar, -1).Value =
            entry.Explanation;

        command.Parameters.Add("@Url", SqlDbType.NVarChar, 2048).Value =
            entry.Url;

        command.Parameters.Add("@MediaType", SqlDbType.NVarChar, 50).Value =
            entry.MediaType;

        command.Parameters.Add("@ServiceVersion", SqlDbType.NVarChar, 50).Value =
            (object?)entry.ServiceVersion ?? DBNull.Value;

        command.Parameters.Add("@PostId", SqlDbType.Int).Value =
            (object?)entry.PostId ?? DBNull.Value;

        command.Parameters.Add("@Permalink", SqlDbType.NVarChar, 2048).Value =
            (object?)entry.Permalink ?? DBNull.Value;

        command.Parameters.Add("@HdUrl", SqlDbType.NVarChar, 2048).Value =
            (object?)entry.HdUrl ?? DBNull.Value;

        command.Parameters.Add("@Credit", SqlDbType.NVarChar, -1).Value =
            (object?)entry.Credit ?? DBNull.Value;

        command.Parameters.Add("@Copyright", SqlDbType.NVarChar, -1).Value =
            (object?)entry.Copyright ?? DBNull.Value;

        command.Parameters.Add("@Alt", SqlDbType.NVarChar, -1).Value =
            (object?)entry.Alt ?? DBNull.Value;

        await connection.OpenAsync();

        int affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }
}