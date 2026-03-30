using WellAnalyzer.Core.Models;

namespace WellAnalyzer.Core.Services;

public class WellValidator
{
    public List<ValidationError> Validate(IEnumerable<Well> wells)
    {
        var errors = new List<ValidationError>();

        foreach (var well in wells)
        {
            foreach (var interval in well.Intervals)
            {
                ValidateDepthFrom(well, interval, errors);
                ValidateDepthOrder(well, interval, errors);
                ValidatePorosity(well, interval, errors);
                ValidateRock(well, interval, errors);
            }

            ValidateOverlaps(well, errors);
        }

        return errors;
    }

    private static void ValidateDepthFrom(
        Well well,
        Interval interval,
        List<ValidationError> errors
    )
    {
        if (interval.DepthFrom < 0)
        {
            errors.Add(new ValidationError(
                interval.LineNumber,
                well.WellId,
                $"DepthFrom ({interval.DepthFrom}) меньше 0."
            ));
        }
    }

    private static void ValidateDepthOrder(
        Well well,
        Interval interval,
        List<ValidationError> errors
    )
    {
        if (interval.DepthFrom >= interval.DepthTo)
        {
            errors.Add(new ValidationError(
                interval.LineNumber,
                well.WellId,
                $"DepthFrom ({interval.DepthFrom}) должен быть меньше DepthTo ({interval.DepthTo})."
            ));
        }
    }

    private static void ValidatePorosity(
        Well well,
        Interval interval,
        List<ValidationError> errors
    )
    {
        if (interval.Porosity < 0 || interval.Porosity > 1)
        {
            errors.Add(new ValidationError(
                interval.LineNumber,
                well.WellId,
                $"Porosity ({interval.Porosity}) вне диапазона [0..1]."
            ));
        }
    }

    private static void ValidateRock(
        Well well,
        Interval interval,
        List<ValidationError> errors
    )
    {
        if (string.IsNullOrWhiteSpace(interval.Rock))
        {
            errors.Add(new ValidationError(
                interval.LineNumber,
                well.WellId,
                "Rock не должен быть пустым."
            ));
        }
    }

    private static void ValidateOverlaps(Well well, List<ValidationError> errors)
    {
        var intervals = well.Intervals;

        for (var i = 0; i < intervals.Count; i++)
        {
            for (var j = i + 1; j < intervals.Count; j++)
            {
                if (intervals[i].IsOverlapping(intervals[j]))
                {
                    errors.Add(new ValidationError(
                        intervals[j].LineNumber,
                        well.WellId,
                        $"Интервал [{intervals[j].DepthFrom}..{intervals[j].DepthTo}] пересекается с [{intervals[i].DepthFrom}..{intervals[i].DepthTo}]."
                    ));
                }
            }
        }
    }
}
