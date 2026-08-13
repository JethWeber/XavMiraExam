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

    /// <summary>Professor precisa de login antes de aceder ao painel.</summary>
    [RelayCommand]
    private void OpenProfessorMode() => _navigation.NavigateToProfessorLogin();

    [RelayCommand]
    private void OpenStudentMode() => _navigation.NavigateToStudentLogin();
}
