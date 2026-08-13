using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;

namespace XavMiraExam.Desktop.ViewModels;

public partial class StudentRegisterViewModel : ObservableObject
{
    private readonly INavigationHost _navigation;
    private readonly IStudentService _studentService;

    [ObservableProperty]
    private string _nome = string.Empty;

    [ObservableProperty]
    private string _sobrenome = string.Empty;

    [ObservableProperty]
    private string _senha = string.Empty;

    [ObservableProperty]
    private string _confirmarSenha = string.Empty;

    [ObservableProperty]
    private string _turma = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Preencha os dados para criar a sua conta.";

    [ObservableProperty]
    private bool _registrationSucceeded;

    public StudentRegisterViewModel(INavigationHost navigation, IStudentService studentService)
    {
        _navigation = navigation;
        _studentService = studentService;
    }

    [RelayCommand]
    private void Register()
    {
        RegistrationSucceeded = false;

        if (Senha != ConfirmarSenha)
        {
            StatusMessage = "A senha e a confirmação não coincidem.";
            return;
        }

        try
        {
            Student student = _studentService.Register(Nome, Sobrenome, Senha, Turma);
            RegistrationSucceeded = true;
            StatusMessage =
                $"Conta criada com sucesso.\n" +
                $"{student.NomeCompleto} — {student.Turma}\n" +
                "Entre com o sobrenome e a senha.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void GoToLogin() => _navigation.NavigateToStudentLogin();

    [RelayCommand]
    private void GoBack() => _navigation.NavigateToHome();
}
