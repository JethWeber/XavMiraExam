using CommunityToolkit.Mvvm.ComponentModel;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Services;
using XavMiraExam.Desktop.Services;

namespace XavMiraExam.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject, INavigationHost
{
    private readonly ExamService _examService;
    private readonly IStudentService _studentService;
    private readonly ExamSessionService _sessionService;
    private readonly EvaluationService _evaluationService;
    private readonly ResultService _resultService;
    private readonly IFilePickerService _filePicker;

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    public MainViewModel(
        ExamService examService,
        IStudentService studentService,
        ExamSessionService sessionService,
        EvaluationService evaluationService,
        ResultService resultService,
        IFilePickerService filePicker)
    {
        _examService = examService;
        _studentService = studentService;
        _sessionService = sessionService;
        _evaluationService = evaluationService;
        _resultService = resultService;
        _filePicker = filePicker;
        NavigateToHome();
    }

    public void NavigateToHome() =>
        CurrentViewModel = new HomeViewModel(this);

    public void NavigateToProfessorLogin() =>
        CurrentViewModel = new ProfessorLoginViewModel(this);

    public void NavigateToProfessor() =>
        CurrentViewModel = new ProfessorViewModel(this, _examService, _sessionService, _filePicker);

    public void NavigateToStudentLogin()
    {
        string? msg = null;
        if (!_sessionService.IsSessionReady)
        {
            msg = "Nenhuma sessão de prova está ativa. Pode criar conta ou entrar; " +
                  "para fazer a prova, o professor precisa de iniciar a sessão.";
        }

        CurrentViewModel = new StudentLoginViewModel(this, _studentService, _sessionService, msg);
    }

    public void NavigateToStudentRegister() =>
        CurrentViewModel = new StudentRegisterViewModel(this, _studentService);

    public void NavigateToExam() =>
        CurrentViewModel = new ExamViewModel(this, _sessionService);

    public void NavigateToExamComplete() =>
        CurrentViewModel = new ExamCompleteViewModel(
            this, _sessionService, _evaluationService, _resultService);
}
