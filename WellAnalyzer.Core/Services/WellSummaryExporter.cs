using System.Text.Json;
using WellAnalyzer.Core.Models;

namespace WellAnalyzer.Core.Services;

public class WellSummaryExporter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };
    
    public async Task ExportAsync(string filePath, IEnumerable<WellSummary> summaries)
    {
        await using var stream = File.Create(filePath);
        await ExportAsync(stream, summaries);
    }

    public async Task ExportAsync(Stream stream, IEnumerable<WellSummary> summaries)
    {
        await JsonSerializer.SerializeAsync(stream, summaries, SerializerOptions);
    }
}