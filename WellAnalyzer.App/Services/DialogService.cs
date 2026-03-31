using Avalonia.Platform.Storage;

namespace WellAnalyzer.App.Services;

using System.Threading.Tasks;

public class DialogService
{
    private readonly IStorageProvider _storageProvider;

    public DialogService(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<IStorageFile?> PickCsvToReadAsync()
    {
        var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите CSV-файл для импорта",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("CSV") { Patterns = ["*.csv"] }
            ]
        });

        return files.Count > 0 ? files[0] : null;
    }

    public async Task<IStorageFile?> PickJsonToWriteAsync()
    {
        return await _storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Сохранить сводку в JSON",
            DefaultExtension = "json",
            FileTypeChoices =
            [
                new FilePickerFileType("JSON") { Patterns = ["*.json"] }
            ]
        });
    }
}
