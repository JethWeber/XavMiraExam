using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XavMiraExam.Core.Models;
using XavMiraExam.Core.Services;
using XavMiraExam.Desktop.Services;

namespace XavMiraExam.Desktop.ViewModels;

public partial class ExamCompleteViewModel : ObservableObject
{
    private readonly INavigationHost _navigation;
    private readonly ResultService _resultService;

    [ObservableProperty] private string _studentName = string.Empty;
    [ObservableProperty] private string _studentCodigo = string.Empty;
    [ObservableProperty] private string _examTitle = string.Empty;
    [ObservableProperty] private int _totalQuestions;
    [ObservableProperty] private int _respondidas;
    [ObservableProperty] private int _naoRespondidas;
    [ObservableProperty] private int _acertos;
    [ObservableProperty] private int _erros;
    [ObservableProperty] private double _percentagem;
    [ObservableProperty] private double _nota;
    [ObservableProperty] private double _notaMaxima;
    [ObservableProperty] private string _percentagemText = string.Empty;
    [ObservableProperty] private string _notaText = string.Empty;
    [ObservableProperty] private bool _hasRespostasErradas;
    [ObservableProperty] private string _pdfStatusMessage = string.Empty;
    [ObservableProperty] private string _pdfPath = string.Empty;
    [ObservableProperty] private bool _pdfGenerated;

    public ObservableCollection<WrongAnswerViewModel> RespostasErradas { get; } = new();

    public ExamResult Result { get; }

    private readonly Exam _exam;
    private readonly Student _student;

    public ExamCompleteViewModel(
        INavigationHost navigation,
        ExamSessionService sessionService,
        EvaluationService evaluationService,
        ResultService resultService)
    {
        _navigation = navigation;
        _resultService = resultService;

        _exam = sessionService.CurrentExam!;
        _student = sessionService.CurrentStudent!;
        var answers = sessionService.Answers;

        Result = evaluationService.Evaluate(_exam, _student, answers);

        StudentName = _student.Nome;
        StudentCodigo = _student.Codigo;
        ExamTitle = _exam.Titulo;
        TotalQuestions = Result.TotalQuestoes;
        Respondidas = answers.Count(a => a.FoiRespondida);
        NaoRespondidas = Result.NaoRespondidas;
        Acertos = Result.Acertos;
        Erros = Result.Erros;
        Percentagem = Result.Percentagem;
        Nota = Result.Nota;
        NotaMaxima = _exam.NotaMaxima;
        PercentagemText = $"{Percentagem:0.##}%";
        NotaText = $"{Nota:0.##} / {NotaMaxima:0.##}";

        foreach (Answer wrong in Result.RespostasErradas)
        {
            Question? question = _exam.Questoes.FirstOrDefault(q => q.Id == wrong.QuestionId);
            if (question is null)
                continue;

            string respostaAluno =
                wrong.AlternativaSelecionada.HasValue &&
                wrong.AlternativaSelecionada.Value >= 0 &&
                wrong.AlternativaSelecionada.Value < question.Alternativas.Count
                    ? question.Alternativas[wrong.AlternativaSelecionada.Value]
                    : "(não respondida)";

            string respostaCorreta = question.Alternativas[question.Correta];

            RespostasErradas.Add(new WrongAnswerViewModel(
                question.Id, question.Pergunta, respostaAluno, respostaCorreta));
        }

        HasRespostasErradas = RespostasErradas.Count > 0;

        TryGeneratePdfAndSave();
    }

    private void TryGeneratePdfAndSave()
    {
        try
        {
            // Documentos/XavMiraExam/Results (Linux e Windows)
            string resultsRoot = AppPaths.ResultsFolder;
            string path = _resultService.SaveResult(_exam, _student, Result, resultsRoot);
            PdfPath = path;
            PdfGenerated = true;
            PdfStatusMessage =
                $"Resultado guardado na base de dados.\n" +
                $"PDF: {path}";
        }
        catch (Exception ex)
        {
            PdfGenerated = false;
            PdfPath = string.Empty;
            PdfStatusMessage = "Erro ao guardar resultado/PDF: " + ex.Message;
        }
    }

    [RelayCommand]
    private void ExportPdf() => TryGeneratePdfAndSave();

    [RelayCommand]
    private void OpenPdfFolder()
    {
        if (string.IsNullOrWhiteSpace(PdfPath) || !File.Exists(PdfPath))
        {
            PdfStatusMessage = "O ficheiro PDF ainda não foi gerado.";
            return;
        }

        try
        {
            string? dir = Path.GetDirectoryName(PdfPath);
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = dir,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            PdfStatusMessage = "Não foi possível abrir a pasta: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Finish() => _navigation.NavigateToHome();
}

public class WrongAnswerViewModel
{
    public WrongAnswerViewModel(int number, string pergunta, string respostaAluno, string respostaCorreta)
    {
        Number = number;
        Pergunta = pergunta;
        RespostaAluno = respostaAluno;
        RespostaCorreta = respostaCorreta;
    }

    public int Number { get; }
    public string Pergunta { get; }
    public string RespostaAluno { get; }
    public string RespostaCorreta { get; }
    public string Header => $"Questão {Number:D2}";
}
