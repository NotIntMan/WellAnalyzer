using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace WellAnalyzer.App.Services;

using System.Threading.Tasks;

public class DialogService
{
    private readonly Window _owner;
    private readonly IStorageProvider _storageProvider;

    public DialogService(Window owner)
    {
        _owner = owner;
        _storageProvider = owner.StorageProvider;
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

    public async Task ShowErrorDialogAsync(string title, string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok, Icon.Error);
        await box.ShowWindowDialogAsync(_owner);
    }
}
