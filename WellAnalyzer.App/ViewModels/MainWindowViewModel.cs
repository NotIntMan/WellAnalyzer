using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WellAnalyzer.Core.Models;
using WellAnalyzer.Core.Services;

namespace WellAnalyzer.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IStorageProvider _storageProvider;
    private readonly CsvParser _csvParser;
    private readonly WellValidator _validator;
    private readonly WellSummaryCalculator _calculator;
    private readonly WellSummaryExporter _exporter;

    [ObservableProperty] private string _statusMessage = "Выберите CSV-файл для импорта.";

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
        IStorageProvider storageProvider,
        CsvParser csvParser,
        WellValidator validator,
        WellSummaryCalculator calculator,
        WellSummaryExporter exporter
    )
    {
        _storageProvider = storageProvider;
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
            var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Выберите CSV-файл",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("CSV") { Patterns = ["*.csv"] }
                ]
            });

            if (files.Count == 0)
            {
                return;
            }

            var filePath = files[0].Path.LocalPath;
            var wells = await _csvParser.ParseAsync(filePath);
            Errors = _validator.Validate(wells);
            Summaries = _calculator.Calculate(wells);

            StatusMessage = $"Импортировано: {filePath}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка импорта: {ex.Message}";
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

            var file = await _storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Сохранить сводку в JSON",
                DefaultExtension = "json",
                FileTypeChoices =
                [
                    new FilePickerFileType("JSON") { Patterns = ["*.json"] }
                ]
            });

            if (file is null)
            {
                return;
            }

            await using var stream = await file.OpenWriteAsync();
            await _exporter.ExportAsync(stream, Summaries);
            StatusMessage = $"Экспортировано: {file.Name}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка экспорта: {ex.Message}";
        }
    }
}