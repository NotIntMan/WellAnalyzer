namespace WellAnalyzer.Core.Models;

public record Interval(double DepthFrom, double DepthTo, string Rock, double Porosity)
{
    public double Width => DepthTo - DepthFrom;
}