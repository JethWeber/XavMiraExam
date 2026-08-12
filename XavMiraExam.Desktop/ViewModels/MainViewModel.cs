using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Services;
using XavMiraExam.Infrastructure.Json;

namespace XavMiraExam.Desktop.ViewModels;

/// <summary>
/// ViewModel da janela principal. Nesta Fase 1, serve apenas para confirmar
/// visualmente que a aplicação Avalonia está corretamente ligada ao Core e à
/// Infrastructure (carregamento + validação de provas). As telas reais de
/// Professor / Aluno / Prova / Resultado serão construídas na Fase 2 em diante.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly ExamService _examService;

    [ObservableProperty]
    private string _statusMessage = "Pronto para verificar o carregamento de uma prova.";

    [ObservableProperty]
    private bool _ultimaVerificacaoComSucesso;

    public MainViewModel()
    {
        IExamJsonReader reader = new ExamJsonReader();
        IExamValidator validator = new ExamValidator();
        _examService = new ExamService(reader, validator);
    }

    [RelayCommand]
    private void VerificarFase1()
    {
        try
        {
            string examsDir = EncontrarPastaExams();
            string caminho = Path.Combine(examsDir, "Informatica.json");

            var exam = _examService.LoadAndValidate(caminho);

            StatusMessage =
                $"Prova \"{exam.Titulo}\" carregada e validada com sucesso.\n" +
                $"{exam.Questoes.Count} questão(ões) — {exam.TempoPorQuestao}s por questão — nota máxima {exam.NotaMaxima}.";
            UltimaVerificacaoComSucesso = true;
        }
        catch (Exception ex)
        {
            StatusMessage = "Falha ao carregar a prova: " + ex.Message;
            UltimaVerificacaoComSucesso = false;
        }
    }

    private static string EncontrarPastaExams()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            string candidate = Path.Combine(dir.FullName, "Exams");
            if (Directory.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("Pasta 'Exams' não encontrada.");
    }
}
