using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XavMiraExam.Core.Services;

namespace XavMiraExam.Desktop.ViewModels;

public partial class ExamCompleteViewModel : ObservableObject
{
    private readonly INavigationHost _navigation;
    private readonly ExamSessionService _sessionService;

    [ObservableProperty]
    private string _studentName = string.Empty;

    [ObservableProperty]
    private string _examTitle = string.Empty;

    [ObservableProperty]
    private int _totalQuestions;

    [ObservableProperty]
    private int _respondidas;

    [ObservableProperty]
    private int _naoRespondidas;

    public ExamCompleteViewModel(INavigationHost navigation, ExamSessionService sessionService)
    {
        _navigation = navigation;
        _sessionService = sessionService;

        var exam = sessionService.CurrentExam!;
        StudentName = sessionService.CurrentStudent!.Nome;
        ExamTitle = exam.Titulo;
        TotalQuestions = exam.Questoes.Count;
        Respondidas = sessionService.Answers.Count(a => a.FoiRespondida);
        NaoRespondidas = sessionService.Answers.Count(a => !a.FoiRespondida);
    }

    [RelayCommand]
    private void Finish() => _navigation.NavigateToHome();
}
