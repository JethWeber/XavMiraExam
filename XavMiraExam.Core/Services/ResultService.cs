using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Services;

/// <summary>
/// Orquestra PDF + persistência SQLite do resultado.
/// </summary>
public class ResultService
{
    private readonly IPdfReportService _pdfReport;
    private readonly IResultStore _resultStore;

    public ResultService(IPdfReportService pdfReport, IResultStore resultStore)
    {
        _pdfReport = pdfReport;
        _resultStore = resultStore;
    }

    /// <summary>
    /// Gera o PDF, grava o resultado na base de dados e devolve o caminho do PDF.
    /// </summary>
    public string SaveResult(
        Exam exam,
        Student student,
        ExamResult result,
        string resultsRootFolder)
    {
        ArgumentNullException.ThrowIfNull(exam);
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(result);
        ArgumentException.ThrowIfNullOrWhiteSpace(resultsRootFolder);

        Directory.CreateDirectory(resultsRootFolder);

        string pdfPath = _pdfReport.GenerateAndSave(exam, student, result, resultsRootFolder);

        // Novo Id em cada gravação para evitar UNIQUE constraint ao regenerar PDF
        result.Id = Guid.NewGuid();

        _resultStore.Save(
            result,
            examTitle: exam.Titulo,
            notaMaxima: exam.NotaMaxima,
            studentCodigo: student.Codigo,
            studentNome: student.NomeCompleto,
            pdfPath: pdfPath);

        return pdfPath;
    }
}
