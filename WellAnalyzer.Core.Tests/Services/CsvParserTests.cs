using WellAnalyzer.Core.Models;
using WellAnalyzer.Core.Services;

namespace WellAnalyzer.Core.Tests.Services;

public class CsvParserTests
{
    private readonly CsvParser _sut = new();

    [Fact]
    public async Task ParseAsync_SingleRow()
    {
        using var reader = new StringReader("A-001;82.10;55.20;10;25;Limestone;0.07");

        var wells = await _sut.ParseAsync(reader);

        wells.Should().BeEquivalentTo([
            new Well("A-001", 82.10, 55.20, [new Interval(1, 10, 25, "Limestone", 0.07)])
        ]);
    }

    [Fact]
    public async Task ParseAsync_ShouldGroupByWellId()
    {
        using var reader = new StringReader(
            "A-001;82.10;55.20;0;10;Sandstone;0.18\n" +
            "A-001;82.10;55.20;10;25;Limestone;0.07\n" +
            "A-002;90.00;60.00;0;15;Shale;0.04");

        var wells = await _sut.ParseAsync(reader);

        wells.Should().HaveCount(2);
        wells[0].WellId.Should().Be("A-001");
        wells[0].Intervals.Should().HaveCount(2);
        wells[1].WellId.Should().Be("A-002");
        wells[1].Intervals.Should().HaveCount(1);
    }

    [Fact]
    public async Task ParseAsync_EmptyInput()
    {
        using var reader = new StringReader("");

        var wells = await _sut.ParseAsync(reader);

        wells.Should().BeEmpty();
    }
}
