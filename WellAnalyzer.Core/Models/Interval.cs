namespace WellAnalyzer.Core.Models;

public record Interval(int LineNumber, double DepthFrom, double DepthTo, string Rock, double Porosity)
{
    public double Width => DepthTo - DepthFrom;

    public bool IsOverlapping(Interval other)
    {
        return DepthFrom < other.DepthTo && other.DepthFrom < DepthTo;
    }
}