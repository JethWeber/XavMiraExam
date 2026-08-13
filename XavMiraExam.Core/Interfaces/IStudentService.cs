using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Interfaces;

/// <summary>
/// Alunos (SQLite). Login: Sobrenome + Senha.
/// </summary>
public interface IStudentService
{
    /// <summary>Autentica por sobrenome + senha. Null se inválido.</summary>
    Student? Authenticate(string sobrenome, string senha);

    /// <summary>Cria conta.</summary>
    Student Register(string nome, string sobrenome, string senha, string? turma = null);

    /// <summary>Lista todos (professor).</summary>
    IReadOnlyList<Student> GetAll();
}
