using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Interfaces;

/// <summary>
/// Contrato para a validação estrutural e semântica de uma prova
/// antes de esta poder ser iniciada (ver secção 16 do plano — Segurança Básica).
/// </summary>
public interface IExamValidator
{
    /// <summary>
    /// Valida uma prova já carregada em memória.
    /// </summary>
    /// <returns>Um <see cref="ValidationResult"/> com o estado da validação e a lista de erros encontrados.</returns>
    ValidationResult Validate(Exam exam);
}

/// <summary>
/// Resultado da validação de uma prova.
/// </summary>
public class ValidationResult
{
    /// <summary>Indica se a prova é válida e pode ser iniciada.</summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>Lista de erros encontrados durante a validação (vazia se válida).</summary>
    public List<string> Errors { get; } = new();

    public void AddError(string message) => Errors.Add(message);
}
