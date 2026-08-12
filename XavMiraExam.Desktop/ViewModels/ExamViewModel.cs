using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XavMiraExam.Core.Services;

namespace XavMiraExam.Desktop.ViewModels;

public partial class ExamViewModel : ObservableObject, IDisposable
{
    private readonly INavigationHost _navigation;
    private readonly ExamSessionService _sessionService;
    private readonly DispatcherTimer _timer;
    private bool _isProcessingAnswer;

    [ObservableProperty]
    private int _questionNumber;

    [ObservableProperty]
    private int _totalQuestions;

    [ObservableProperty]
    private string _progressText = string.Empty;

    [ObservableProperty]
    private string _questionText = string.Empty;

    [ObservableProperty]
    private int _secondsRemaining;

    [ObservableProperty]
    private string _timerText = string.Empty;

    [ObservableProperty]
    private string _studentName = string.Empty;

    [ObservableProperty]
    private string _examTitle = string.Empty;

    [ObservableProperty]
    private bool _canAnswer = true;

    public ObservableCollection<AlternativeViewModel> Alternatives { get; } = new();

    public ExamViewModel(INavigationHost navigation, ExamSessionService sessionService)
    {
        _navigation = navigation;
        _sessionService = sessionService;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1),
        };
        _timer.Tick += OnTimerTick;

        LoadCurrentQuestion();
    }

    private void LoadCurrentQuestion()
    {
        _timer.Stop();
        _isProcessingAnswer = false;
        CanAnswer = true;

        var exam = _sessionService.CurrentExam!;
        var question = _sessionService.GetCurrentQuestion();

        StudentName = _sessionService.CurrentStudent!.Nome;
        ExamTitle = exam.Titulo;
        TotalQuestions = exam.Questoes.Count;
        QuestionNumber = _sessionService.CurrentQuestionIndex + 1;
        ProgressText = $"QUESTÃO {QuestionNumber:D2} / {TotalQuestions:D2}";
        QuestionText = question.Pergunta;

        Alternatives.Clear();
        for (int i = 0; i < question.Alternativas.Count; i++)
        {
            Alternatives.Add(new AlternativeViewModel(i, GetLetter(i), question.Alternativas[i]));
        }

        SecondsRemaining = exam.TempoPorQuestao;
        UpdateTimerText();
        _timer.Start();
    }

    [RelayCommand]
    private void SelectAnswer(int index)
    {
        if (!CanAnswer || _isProcessingAnswer)
            return;

        SubmitAnswer(index);
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        SecondsRemaining--;
        UpdateTimerText();

        if (SecondsRemaining <= 0)
        {
            _timer.Stop();
            SubmitAnswer(null);
        }
    }

    private void SubmitAnswer(int? selectedIndex)
    {
        if (_isProcessingAnswer)
            return;

        _isProcessingAnswer = true;
        CanAnswer = false;
        _timer.Stop();

        _sessionService.RecordAnswer(selectedIndex);

        if (_sessionService.AdvanceToNextQuestion())
            LoadCurrentQuestion();
        else
            _navigation.NavigateToExamComplete();
    }

    private void UpdateTimerText() =>
        TimerText = $"{Math.Max(SecondsRemaining, 0):D2}s";

    private static string GetLetter(int index) =>
        ((char)('A' + index)).ToString();

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
    }
}

public class AlternativeViewModel
{
    public AlternativeViewModel(int index, string letter, string text)
    {
        Index = index;
        Letter = letter;
        Text = text;
    }

    public int Index { get; }
    public string Letter { get; }
    public string Text { get; }
    public string Label => $"[ {Letter} ] {Text}";
}
