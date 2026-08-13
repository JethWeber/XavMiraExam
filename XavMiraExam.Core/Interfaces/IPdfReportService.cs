using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Interfaces;

/// <summary>
/// Geração e gravação do relatório PDF do aluno.
/// </summary>
public interface IPdfReportService
{
    /// <summary>
    /// Gera o PDF em <paramref name="resultsRootFolder"/>/{NomeAluno}/Resultado_{Prova}.pdf
    /// e devolve o caminho completo.
    /// </summary>
    string GenerateAndSave(
        Exam exam,
        Student student,
        ExamResult result,
        string resultsRootFolder);
}
