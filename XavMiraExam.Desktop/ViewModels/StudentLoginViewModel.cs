using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;
using XavMiraExam.Core.Services;

namespace XavMiraExam.Desktop.ViewModels;

public partial class StudentLoginViewModel : ObservableObject
{
    private readonly INavigationHost _navigation;
    private readonly IStudentService _studentService;
    private readonly ExamSessionService _sessionService;

    [ObservableProperty]
    private string _sobrenome = string.Empty;

    [ObservableProperty]
    private string _senha = string.Empty;

    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private bool _isStudentIdentified;

    [ObservableProperty]
    private string _studentName = string.Empty;

    [ObservableProperty]
    private string _studentTurma = string.Empty;

    [ObservableProperty]
    private string _examTitle = string.Empty;

    private Student? _identifiedStudent;

    public StudentLoginViewModel(
        INavigationHost navigation,
        IStudentService studentService,
        ExamSessionService sessionService,
        string? initialMessage = null)
    {
        _navigation = navigation;
        _studentService = studentService;
        _sessionService = sessionService;

        _statusMessage = initialMessage ??
                         "Introduza o sobrenome e a senha. Se ainda não tem conta, crie uma.";

        if (_sessionService.CurrentExam is not null)
            ExamTitle = _sessionService.CurrentExam.Titulo;
    }

    [RelayCommand]
    private void IdentifyStudent()
    {
        IsStudentIdentified = false;
        _identifiedStudent = null;

        if (string.IsNullOrWhiteSpace(Sobrenome))
        {
            StatusMessage = "Introduza o sobrenome.";
            return;
        }

        if (string.IsNullOrEmpty(Senha))
        {
            StatusMessage = "Introduza a senha.";
            return;
        }

        Student? student = _studentService.Authenticate(Sobrenome, Senha);
        if (student is null)
        {
            StatusMessage =
                "Credenciais inválidas. Verifique o sobrenome e a senha, ou crie uma conta.";
            return;
        }

        _identifiedStudent = student;
        StudentName = student.NomeCompleto;
        StudentTurma = student.Turma;
        IsStudentIdentified = true;

        if (!_sessionService.IsSessionReady)
        {
            StatusMessage =
                "Conta identificada. Ainda não há sessão de prova ativa — peça ao professor para iniciar a prova.";
            return;
        }

        StatusMessage = "Confirme a sua identificação para iniciar a prova.";
    }

    [RelayCommand]
    private void StartExam()
    {
        if (_identifiedStudent is null)
        {
            StatusMessage = "Identifique-se primeiro.";
            return;
        }

        if (!_sessionService.IsSessionReady)
        {
            StatusMessage = "A sessão de prova não está disponível. Contacte o professor.";
            return;
        }

        _sessionService.BeginExam(_identifiedStudent);
        _navigation.NavigateToExam();
    }

    [RelayCommand]
    private void GoToRegister() => _navigation.NavigateToStudentRegister();

    [RelayCommand]
    private void GoBack() => _navigation.NavigateToHome();
}
