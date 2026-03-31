using FluentAssertions.Equivalency;
using WellAnalyzer.Core.Models;
using WellAnalyzer.Core.Services;

namespace WellAnalyzer.Core.Tests.Services;

public class WellSummaryCalculatorTests
{
    private const double Precision = 0.000001;

    private readonly WellSummaryCalculator _sut = new();

    [Fact]
    public void Calculate_WhenWellHasIntervals()
    {
        var wells = new[]
        {
            new Well("A-001", 82.1, 55.2,
            [
                new Interval(1, 0, 10, "Sandstone", 0.18),
                new Interval(2, 10, 25, "Limestone", 0.07)
            ])
        };

        var summaries = _sut.Calculate(wells);

        summaries.Should().BeEquivalentTo(
            [
                new WellSummary("A-001", 25, 2, 0.114, "Limestone")
            ],
            AssertionOptions
        );
    }

    [Fact]
    public void Calculate_WhenWellHasNoIntervals()
    {
        var wells = new[]
        {
            new Well("A-002", 90, 60, [])
        };

        var summaries = _sut.Calculate(wells);

        summaries.Should().BeEquivalentTo([new WellSummary("A-002", 0, 0, 0, string.Empty)]);
    }

    [Fact]
    public void Calculate_WhenDominantRockIsDerivedByTotalIntervalWidth()
    {
        var wells = new[]
        {
            new Well("A-003", 100.1, 72.5,
            [
                new Interval(1, 0, 5, "Shale", 0.04),
                new Interval(2, 5, 20, "Sandstone", 0.22),
                new Interval(3, 20, 25, "Shale", 0.05)
            ])
        };

        var summaries = _sut.Calculate(wells);

        summaries.Should().BeEquivalentTo(
            [
                new WellSummary("A-003", 25, 3, 0.15, "Sandstone")
            ],
            AssertionOptions
        );
    }

    [Fact]
    public void Calculate_WhenIntervalsHaveGap()
    {
        var wells = new[]
        {
            new Well("A-004", 110, 80,
            [
                new Interval(1, 0, 10, "Sandstone", 0.2),
                new Interval(2, 20, 30, "Limestone", 0.4)
            ])
        };

        var summaries = _sut.Calculate(wells);

        summaries.Should().BeEquivalentTo(
            [
                new WellSummary("A-004", 30, 2, 0.3, "Limestone")
            ],
            AssertionOptions
        );
    }

    private static EquivalencyAssertionOptions<T> AssertionOptions<T>(EquivalencyAssertionOptions<T> options)
    {
        return options
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, Precision))
            .WhenTypeIs<double>();
    }
}
