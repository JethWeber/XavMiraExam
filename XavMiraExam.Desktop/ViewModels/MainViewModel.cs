using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Services;
using XavMiraExam.Desktop.Services;

namespace XavMiraExam.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject, INavigationHost
{
    private readonly ExamService _examService;
    private readonly IStudentService _studentService;
    private readonly ExamSessionService _sessionService;
    private readonly IFilePickerService _filePicker;

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    public MainViewModel(
        ExamService examService,
        IStudentService studentService,
        ExamSessionService sessionService,
        IFilePickerService filePicker)
    {
        _examService = examService;
        _studentService = studentService;
        _sessionService = sessionService;
        _filePicker = filePicker;
        NavigateToHome();
    }

    public void NavigateToHome() =>
        CurrentViewModel = new HomeViewModel(this);

    public void NavigateToProfessor() =>
        CurrentViewModel = new ProfessorViewModel(this, _examService, _sessionService, _filePicker);

    public void NavigateToStudentLogin()
    {
        if (!_sessionService.IsSessionReady)
        {
            CurrentViewModel = new StudentLoginViewModel(
                this,
                _studentService,
                _sessionService,
                "Nenhuma sessão de prova está ativa. Peça ao professor para importar e iniciar a prova.");
            return;
        }

        CurrentViewModel = new StudentLoginViewModel(this, _studentService, _sessionService);
    }

    public void NavigateToExam() =>
        CurrentViewModel = new ExamViewModel(this, _sessionService);

    public void NavigateToExamComplete() =>
        CurrentViewModel = new ExamCompleteViewModel(this, _sessionService);
}
