using WellAnalyzer.Core.Models;

namespace WellAnalyzer.Core.Services;

public class WellSummaryCalculator
{
    public List<WellSummary> Calculate(IEnumerable<Well> wells)
    {
        return wells.Select(CalculateForWell).ToList();
    }

    private static WellSummary CalculateForWell(Well well)
    {
        var intervals = well.Intervals;
        var totalDepth = intervals.Count > 0
            ? intervals.Max(i => i.DepthTo)
            : 0;
        var intervalCount = intervals.Count;
        var totalIntervalWidth = intervals.Sum(i => i.Width);

        var weightedAveragePorosity = totalIntervalWidth > 0
            ? intervals.Sum(i => i.Porosity * i.Width) / totalIntervalWidth
            : 0;

        var dominantRock = intervals
            .GroupBy(i => i.Rock)
            .Select(g => new { Rock = g.Key, TotalWidth = g.Sum(i => i.Width) })
            .OrderByDescending(x => x.TotalWidth)
            .ThenBy(x => x.Rock, StringComparer.InvariantCultureIgnoreCase)
            .Select(x => x.Rock)
            .FirstOrDefault(string.Empty);

        return new WellSummary(
            well.WellId,
            totalDepth,
            intervalCount,
            weightedAveragePorosity,
            dominantRock
        );
    }
}