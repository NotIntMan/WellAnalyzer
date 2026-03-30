namespace WellAnalyzer.Core.Models;

public record Well(string WellId, double X, double Y, IReadOnlyList<Interval> Intervals);