using WellAnalyzer.Core.Models;
using WellAnalyzer.Core.Services;

namespace WellAnalyzer.Core.Tests.Services;

public class WellValidatorTests
{
    private readonly WellValidator _sut = new();

    [Fact]
    public void Validate_WhenDataIsValid()
    {
        var wells = new[]
        {
            CreateWell("A-001",
                new Interval(1, 0, 10, "Sandstone", 0.18),
                new Interval(2, 10, 25, "Limestone", 0.07)
            )
        };

        var errors = _sut.Validate(wells);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WhenDepthFromIsNegative()
    {
        var wells = new[]
        {
            CreateWell("A-001", new Interval(3, -1, 10, "Sandstone", 0.18))
        };

        var errors = _sut.Validate(wells);

        errors.Should().ContainSingle(e =>
            e.LineNumber == 3 &&
            e.WellId == "A-001" &&
            e.Description.Contains("меньше 0")
        );
    }

    [Fact]
    public void Validate_WhenDepthFromIsNotLessThanDepthTo()
    {
        var wells = new[]
        {
            CreateWell("A-001", new Interval(4, 10, 10, "Sandstone", 0.18))
        };

        var errors = _sut.Validate(wells);

        errors.Should().ContainSingle(e =>
            e.LineNumber == 4 &&
            e.WellId == "A-001" &&
            e.Description.Contains("должен быть меньше")
        );
    }

    [Fact]
    public void Validate_WhenPorosityIsOutOfRange()
    {
        var wells = new[]
        {
            CreateWell("A-001", new Interval(5, 0, 10, "Sandstone", 1.2))
        };

        var errors = _sut.Validate(wells);

        errors.Should().ContainSingle(e =>
            e.LineNumber == 5 &&
            e.WellId == "A-001" &&
            e.Description.Contains("вне диапазона")
        );
    }

    [Fact]
    public void Validate_WhenRockIsEmpty()
    {
        var wells = new[]
        {
            CreateWell("A-001", new Interval(6, 0, 10, " ", 0.18))
        };

        var errors = _sut.Validate(wells);

        errors.Should().ContainSingle(e =>
            e.LineNumber == 6 &&
            e.WellId == "A-001" &&
            e.Description.Contains("Rock не должен быть пустым")
        );
    }

    [Fact]
    public void Validate_WhenIntervalsOverlap()
    {
        var wells = new[]
        {
            CreateWell("A-001",
                new Interval(7, 0, 10, "Sandstone", 0.18),
                new Interval(8, 9, 20, "Limestone", 0.07)
            )
        };

        var errors = _sut.Validate(wells);

        errors.Should().ContainSingle(e =>
            e.LineNumber == 8 &&
            e.WellId == "A-001" &&
            e.Description.Contains("пересекается"));
    }

    private static Well CreateWell(string wellId, params Interval[] intervals)
    {
        return new Well(wellId, 82.1, 55.2, intervals);
    }
}
