namespace WellAnalyzer.Core.Models;

public record WellSummary(
    string WellId,
    double TotalDepth,
    int IntervalCount,
    double WeightedAveragePorosity,
    string DominantRock
);