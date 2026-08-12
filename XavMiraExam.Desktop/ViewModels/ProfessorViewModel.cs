using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XavMiraExam.Core.Models;
using XavMiraExam.Core.Services;
using XavMiraExam.Desktop.Services;

namespace XavMiraExam.Desktop.ViewModels;

public partial class ProfessorViewModel : ObservableObject
{
    private readonly INavigationHost _navigation;
    private readonly ExamService _examService;
    private readonly ExamSessionService _sessionService;
    private readonly IFilePickerService _filePicker;

    [ObservableProperty]
    private string _statusMessage = "Importe um ficheiro JSON de prova para começar.";

    [ObservableProperty]
    private bool _isExamLoaded;

    [ObservableProperty]
    private bool _isSessionStarted;

    [ObservableProperty]
    private string _examTitle = string.Empty;

    [ObservableProperty]
    private int _questionCount;

    [ObservableProperty]
    private int _tempoPorQuestao = 10;

    [ObservableProperty]
    private double _notaMaxima;

    public ObservableCollection<string> ValidationErrors { get; } = new();

    public bool HasValidationErrors => ValidationErrors.Count > 0;

    public ProfessorViewModel(
        INavigationHost navigation,
        ExamService examService,
        ExamSessionService sessionService,
        IFilePickerService filePicker)
    {
        _navigation = navigation;
        _examService = examService;
        _sessionService = sessionService;
        _filePicker = filePicker;
    }

    [RelayCommand]
    private async Task ImportExamAsync()
    {
        ValidationErrors.Clear();
        IsSessionStarted = false;
        OnPropertyChanged(nameof(HasValidationErrors));

        string? filePath = await _filePicker.PickJsonFileAsync();
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        try
        {
            Exam exam = _examService.LoadAndValidate(filePath);
            _sessionService.ConfigureExam(exam);

            ExamTitle = exam.Titulo;
            QuestionCount = exam.Questoes.Count;
            TempoPorQuestao = exam.TempoPorQuestao;
            NotaMaxima = exam.NotaMaxima;
            IsExamLoaded = true;

            StatusMessage =
                $"Prova \"{exam.Titulo}\" importada e validada com sucesso.\n" +
                "Confirme o tempo por questão e inicie a sessão.";
        }
        catch (ExamValidationException ex)
        {
            IsExamLoaded = false;
            foreach (string error in ex.Errors)
                ValidationErrors.Add(error);
            OnPropertyChanged(nameof(HasValidationErrors));

            StatusMessage = "A prova importada não passou na validação.";
        }
        catch (Exception ex)
        {
            IsExamLoaded = false;
            StatusMessage = "Erro ao importar a prova: " + ex.Message;
        }
    }

    [RelayCommand]
    private void StartSession()
    {
        if (!IsExamLoaded || _sessionService.CurrentExam is null)
        {
            StatusMessage = "Importe e valide uma prova antes de iniciar a sessão.";
            return;
        }

        if (TempoPorQuestao <= 0)
        {
            StatusMessage = "O tempo por questão deve ser maior que zero.";
            return;
        }

        _sessionService.ConfigureExam(_sessionService.CurrentExam, TempoPorQuestao);
        _sessionService.StartSession();
        IsSessionStarted = true;

        StatusMessage =
            "Sessão iniciada. Os alunos já podem entrar com a credencial.\n" +
            "Pode passar o computador ao aluno ou abrir o modo Aluno.";
    }

    [RelayCommand]
    private void OpenStudentLogin() => _navigation.NavigateToStudentLogin();

    [RelayCommand]
    private void GoBack() => _navigation.NavigateToHome();
}
