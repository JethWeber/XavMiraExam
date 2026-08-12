using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Services;

/// <summary>
/// Gere o estado de uma sessão de prova: prova carregada, aluno identificado,
/// questão atual e registo das respostas (Fase 2).
/// </summary>
public class ExamSessionService
{
    private Exam? _exam;
    private Student? _student;
    private readonly List<Answer> _answers = new();
    private int _currentQuestionIndex;
    private bool _sessionStarted;
    private bool _examInProgress;
    private DateTime _questionStartedAt;

    public Exam? CurrentExam => _exam;
    public Student? CurrentStudent => _student;
    public bool IsSessionReady => _exam is not null && _sessionStarted;
    public bool IsExamInProgress => _examInProgress;
    public int CurrentQuestionIndex => _currentQuestionIndex;
    public IReadOnlyList<Answer> Answers => _answers;

    public bool IsExamComplete =>
        _exam is not null && !_examInProgress && _answers.Count == _exam.Questoes.Count;

    public void ConfigureExam(Exam exam, int? tempoPorQuestaoOverride = null)
    {
        ArgumentNullException.ThrowIfNull(exam);

        if (tempoPorQuestaoOverride.HasValue)
            exam.TempoPorQuestao = tempoPorQuestaoOverride.Value;

        _exam = exam;
        ResetSessionState();
    }

    public void StartSession()
    {
        if (_exam is null)
            throw new InvalidOperationException("Nenhuma prova foi carregada.");

        _sessionStarted = true;
    }

    public void BeginExam(Student student)
    {
        if (!_sessionStarted || _exam is null)
            throw new InvalidOperationException("A sessão de prova ainda não foi iniciada pelo professor.");

        ArgumentNullException.ThrowIfNull(student);

        _student = student;
        _currentQuestionIndex = 0;
        _answers.Clear();
        _examInProgress = true;
        _questionStartedAt = DateTime.UtcNow;
    }

    public Question GetCurrentQuestion()
    {
        if (_exam is null || _currentQuestionIndex >= _exam.Questoes.Count)
            throw new InvalidOperationException("Não há questão atual disponível.");

        return _exam.Questoes[_currentQuestionIndex];
    }

    public void RecordAnswer(int? selectedIndex)
    {
        if (!_examInProgress || _student is null || _exam is null)
            throw new InvalidOperationException("A prova não está em curso.");

        var question = GetCurrentQuestion();
        double timeUsed = (DateTime.UtcNow - _questionStartedAt).TotalSeconds;
        timeUsed = Math.Min(timeUsed, _exam.TempoPorQuestao);

        _answers.Add(new Answer
        {
            QuestionId = question.Id,
            StudentId = _student.Id,
            AlternativaSelecionada = selectedIndex,
            Correta = selectedIndex.HasValue && question.EhCorreta(selectedIndex.Value),
            TempoUtilizado = timeUsed,
        });
    }

    /// <summary>
    /// Avança para a próxima questão. Devolve true se existir próxima questão;
    /// false quando a prova termina.
    /// </summary>
    public bool AdvanceToNextQuestion()
    {
        if (_exam is null)
            return false;

        _currentQuestionIndex++;

        if (_currentQuestionIndex >= _exam.Questoes.Count)
        {
            _examInProgress = false;
            return false;
        }

        _questionStartedAt = DateTime.UtcNow;
        return true;
    }

    public void ResetSession()
    {
        _exam = null;
        ResetSessionState();
    }

    private void ResetSessionState()
    {
        _student = null;
        _answers.Clear();
        _currentQuestionIndex = 0;
        _sessionStarted = false;
        _examInProgress = false;
    }
}
