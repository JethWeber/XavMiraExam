using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Interfaces;

/// <summary>
/// Contrato para identificação de alunos pela credencial/código.
/// </summary>
public interface IStudentService
{
    /// <summary>
    /// Procura um aluno pelo código introduzido. Devolve null se não existir.
    /// </summary>
    Student? FindByCodigo(string codigo);
}
