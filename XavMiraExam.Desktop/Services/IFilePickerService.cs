namespace XavMiraExam.Desktop.Services;

public interface IFilePickerService
{
    Task<string?> PickJsonFileAsync();
}
