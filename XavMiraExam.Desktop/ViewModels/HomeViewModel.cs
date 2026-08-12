using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace XavMiraExam.Desktop.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly INavigationHost _navigation;

    public HomeViewModel(INavigationHost navigation)
    {
        _navigation = navigation;
    }

    [RelayCommand]
    private void OpenProfessorMode() => _navigation.NavigateToProfessor();

    [RelayCommand]
    private void OpenStudentMode() => _navigation.NavigateToStudentLogin();
}
