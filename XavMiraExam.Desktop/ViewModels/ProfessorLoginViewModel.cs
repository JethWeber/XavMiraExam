using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XavMiraExam.Core.Services;

namespace XavMiraExam.Desktop.ViewModels;

/// <summary>
/// Login obrigatório do professor antes do modo Professor.
/// Credenciais embutidas: Santareno / (hash de Jeth@2026_hidden).
/// </summary>
public partial class ProfessorLoginViewModel : ObservableObject
{
    private readonly INavigationHost _navigation;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Introduza as credenciais de professor.";

    [ObservableProperty]
    private bool _hasError;

    public ProfessorLoginViewModel(INavigationHost navigation)
    {
        _navigation = navigation;
    }

    [RelayCommand]
    private void Login()
    {
        HasError = false;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrEmpty(Password))
        {
            HasError = true;
            StatusMessage = "Preencha utilizador e senha.";
            return;
        }

        if (!ProfessorAuth.Validate(Username, Password))
        {
            HasError = true;
            StatusMessage = "Credenciais de professor inválidas.";
            Password = string.Empty;
            return;
        }

        StatusMessage = "Acesso autorizado.";
        _navigation.NavigateToProfessor();
    }

    [RelayCommand]
    private void GoBack() => _navigation.NavigateToHome();
}
