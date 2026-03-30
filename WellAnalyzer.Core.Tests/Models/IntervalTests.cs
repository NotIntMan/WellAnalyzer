using WellAnalyzer.Core.Models;

namespace WellAnalyzer.Core.Tests.Models;

public class IntervalTests
{
    [Fact]
    public void IsOverlapping_WhenIntervalsIntersect()
    {
        var left = new Interval(1, 10, 20, "RockA", 0.2);
        var right = new Interval(2, 15, 25, "RockB", 0.3);

        left.IsOverlapping(right).Should().BeTrue();
    }

    [Fact]
    public void IsOverlapping_WhenIntervalsOnlyTouchBoundary()
    {
        var left = new Interval(1, 10, 20, "RockA", 0.2);
        var right = new Interval(2, 20, 30, "RockB", 0.3);

        left.IsOverlapping(right).Should().BeFalse();
    }

    [Fact]
    public void IsOverlapping_WhenIntervalsAreSeparated()
    {
        var left = new Interval(1, 10, 20, "RockA", 0.2);
        var right = new Interval(2, 21, 30, "RockB", 0.3);

        left.IsOverlapping(right).Should().BeFalse();
    }
}
