using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WellAnalyzer.App.Services;
using WellAnalyzer.Core.Models;
using WellAnalyzer.Core.Services;

namespace WellAnalyzer.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly DialogService _dialogService;
    private readonly CsvParser _csvParser;
    private readonly WellValidator _validator;
    private readonly WellSummaryCalculator _calculator;
    private readonly WellSummaryExporter _exporter;

    [ObservableProperty] private string _statusMessage = "Выберите CSV-файл для импорта.";

    [ObservableProperty] private bool _isStatusError;

    [ObservableProperty] private IReadOnlyList<WellSummary> _summaries = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasErrors))]
    [NotifyPropertyChangedFor(nameof(SummariesRowHeight))]
    [NotifyPropertyChangedFor(nameof(ErrorsRowHeight))]
    private IReadOnlyList<ValidationError> _errors = [];

    public bool HasErrors => Errors.Count > 0;

    public GridLength SummariesRowHeight =>
        HasErrors
            ? new GridLength(3, GridUnitType.Star)
            : new GridLength(1, GridUnitType.Star);

    public GridLength ErrorsRowHeight =>
        HasErrors
            ? new GridLength(2, GridUnitType.Star)
            : new GridLength(0);

    public MainWindowViewModel(
        DialogService dialogService,
        CsvParser csvParser,
        WellValidator validator,
        WellSummaryCalculator calculator,
        WellSummaryExporter exporter
    )
    {
        _dialogService = dialogService;
        _csvParser = csvParser;
        _validator = validator;
        _calculator = calculator;
        _exporter = exporter;
    }

    [RelayCommand]
    private async Task ImportCsvAsync()
    {
        try
        {
            var file = await _dialogService.PickCsvToReadAsync();

            if (file is null)
            {
                return;
            }

            var filePath = file.Path.LocalPath;

            var (summaries, errors) = await Task.Run(async () =>
            {
                var wells = await _csvParser.ParseAsync(filePath);
                var summaries = _calculator.Calculate(wells);
                var errors = _validator.Validate(wells);
                return (summaries, errors);
            });

            Summaries = summaries;
            Errors = errors;
            IsStatusError = false;
            StatusMessage = $"Импортировано: {filePath}";
        }
        catch (Exception ex)
        {
            IsStatusError = true;
            StatusMessage = $"Ошибка импорта: {ex.Message}";
            await _dialogService.ShowErrorDialogAsync("Ошибка импорта", ex.Message);
        }
    }

    [RelayCommand]
    private async Task ExportJsonAsync()
    {
        try
        {
            if (Summaries.Count == 0)
            {
                return;
            }

            var file = await _dialogService.PickJsonToWriteAsync();

            if (file is null)
            {
                return;
            }

            await Task.Run(async () =>
            {
                await using var stream = await file.OpenWriteAsync();
                await _exporter.ExportAsync(stream, Summaries);
            });

            IsStatusError = false;
            StatusMessage = $"Экспортировано: {file.Name}";
        }
        catch (Exception ex)
        {
            IsStatusError = true;
            StatusMessage = $"Ошибка экспорта: {ex.Message}";
            await _dialogService.ShowErrorDialogAsync("Ошибка экспорта", ex.Message);
        }
    }
}