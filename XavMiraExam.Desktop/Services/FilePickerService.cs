using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace XavMiraExam.Desktop.Services;

public class FilePickerService : IFilePickerService
{
    private Window? _owner;

    public void SetOwner(Window window) => _owner = window;

    public async Task<string?> PickJsonFileAsync()
    {
        if (_owner is null)
            return null;

        var files = await _owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Selecionar ficheiro de prova (.json)",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Ficheiros JSON")
                {
                    Patterns = ["*.json"],
                },
            ],
        });

        if (files.Count == 0)
            return null;

        return files[0].TryGetLocalPath();
    }
}
