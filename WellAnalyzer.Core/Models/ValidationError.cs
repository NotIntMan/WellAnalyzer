namespace WellAnalyzer.Core.Models;

public record ValidationError(int LineNumber, string WellId, string Description);