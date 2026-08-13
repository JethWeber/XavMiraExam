using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Interfaces;

/// <summary>
/// Persistência dos resultados de prova (SQLite).
/// </summary>
public interface IResultStore
{
    /// <summary>
    /// Guarda (ou atualiza) um resultado de prova e devolve o registo com Id.
    /// </summary>
    ExamResult Save(
        ExamResult result,
        string examTitle,
        double notaMaxima,
        string studentCodigo,
        string studentNome,
        string? pdfPath);

    /// <summary>
    /// Lista resultados de um aluno pelo código.
    /// </summary>
    IReadOnlyList<StoredExamResult> GetByStudentCodigo(string codigo);

    /// <summary>
    /// Lista todos os resultados (modo professor).
    /// </summary>
    IReadOnlyList<StoredExamResult> GetAll();
}

/// <summary>
/// Resultado persistido com metadados extra para listagens.
/// </summary>
public class StoredExamResult
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentCodigo { get; set; } = string.Empty;
    public string StudentNome { get; set; } = string.Empty;
    public Guid ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public int TotalQuestoes { get; set; }
    public int Acertos { get; set; }
    public int Erros { get; set; }
    public int NaoRespondidas { get; set; }
    public double Percentagem { get; set; }
    public double Nota { get; set; }
    public double NotaMaxima { get; set; }
    public DateTime DataRealizacao { get; set; }
    public string? PdfPath { get; set; }
}
