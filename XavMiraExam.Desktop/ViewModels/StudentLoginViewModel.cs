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
    private string _codigo = string.Empty;

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
                         "Introduza a sua credencial para iniciar a prova.";

        if (_sessionService.CurrentExam is not null)
            ExamTitle = _sessionService.CurrentExam.Titulo;
    }

    [RelayCommand]
    private void IdentifyStudent()
    {
        IsStudentIdentified = false;
        _identifiedStudent = null;

        if (!_sessionService.IsSessionReady)
        {
            StatusMessage = "Nenhuma sessão de prova está ativa. Contacte o professor.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Codigo))
        {
            StatusMessage = "Introduza a sua credencial.";
            return;
        }

        Student? student = _studentService.FindByCodigo(Codigo);
        if (student is null)
        {
            StatusMessage = "Credencial não encontrada. Verifique o código e tente novamente.";
            return;
        }

        _identifiedStudent = student;
        StudentName = student.Nome;
        StudentTurma = student.Turma;
        IsStudentIdentified = true;
        StatusMessage = "Confirme a sua identificação para iniciar a prova.";
    }

    [RelayCommand]
    private void StartExam()
    {
        if (_identifiedStudent is null)
        {
            StatusMessage = "Identifique-se primeiro com a credencial.";
            return;
        }

        if (!_sessionService.IsSessionReady)
        {
            StatusMessage = "A sessão de prova não está disponível.";
            return;
        }

        _sessionService.BeginExam(_identifiedStudent);
        _navigation.NavigateToExam();
    }

    [RelayCommand]
    private void GoBack() => _navigation.NavigateToHome();
}
