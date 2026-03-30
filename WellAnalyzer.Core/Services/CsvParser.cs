using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using WellAnalyzer.Core.Models;

namespace WellAnalyzer.Core.Services;

public class CsvParser
{
    private readonly CsvConfiguration _config = new(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        HasHeaderRecord = false,
    };

    public async Task<List<Well>> ParseAsync(string filePath)
    {
        using var reader = new StreamReader(filePath);
        return await ParseAsync(reader);
    }

    public async Task<List<Well>> ParseAsync(TextReader reader)
    {
        using var csv = new CsvReader(reader, _config);

        var rows = new List<WellCsvRow>();
        await foreach (var row in csv.GetRecordsAsync<WellCsvRow>())
        {
            rows.Add(row);
        }

        return rows
            .GroupBy(r => r.WellId)
            .Select(g =>
            {
                var first = g.First();
                var intervals = g.Select(r => new Interval(r.DepthFrom, r.DepthTo, r.Rock, r.Porosity)).ToList();
                return new Well(first.WellId, first.X, first.Y, intervals);
            })
            .ToList();
    }

    private class WellCsvRow
    {
        [Index(0)] public string WellId { get; set; } = "";
        [Index(1)] public double X { get; set; }
        [Index(2)] public double Y { get; set; }
        [Index(3)] public double DepthFrom { get; set; }
        [Index(4)] public double DepthTo { get; set; }
        [Index(5)] public string Rock { get; set; } = "";
        [Index(6)] public double Porosity { get; set; }
    }
}
