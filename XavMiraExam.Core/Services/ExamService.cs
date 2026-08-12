using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Services;

/// <summary>
/// Serviço responsável por carregar uma prova a partir de um ficheiro JSON
/// e garantir que ela é válida antes de poder ser usada numa sessão de prova.
/// </summary>
public class ExamService
{
    private readonly IExamJsonReader _reader;
    private readonly IExamValidator _validator;

    public ExamService(IExamJsonReader reader, IExamValidator validator)
    {
        _reader = reader;
        _validator = validator;
    }

    /// <summary>
    /// Carrega e valida uma prova a partir de um ficheiro no disco.
    /// </summary>
    /// <exception cref="ExamValidationException">
    /// Lançada quando o ficheiro é lido com sucesso mas a prova não passa na validação.
    /// </exception>
    public Exam LoadAndValidate(string filePath)
    {
        Exam exam = _reader.ReadFromFile(filePath);

        ValidationResult validation = _validator.Validate(exam);
        if (!validation.IsValid)
            throw new ExamValidationException(validation.Errors);

        return exam;
    }
}

/// <summary>
/// Exceção lançada quando uma prova carregada não passa na validação estrutural/semântica.
/// </summary>
public class ExamValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public ExamValidationException(IEnumerable<string> errors)
        : base("A prova não é válida: " + string.Join(" | ", errors))
    {
        Errors = errors.ToList();
    }
}
