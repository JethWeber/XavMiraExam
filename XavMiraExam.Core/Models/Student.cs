namespace XavMiraExam.Core.Models;

/// <summary>
/// Aluno. Login: Sobrenome + Senha.
/// </summary>
public class Student
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Código interno gerado.</summary>
    public string Codigo { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Sobrenome { get; set; } = string.Empty;

    /// <summary>Hash da senha (nunca texto claro).</summary>
    public string SenhaHash { get; set; } = string.Empty;

    public string Turma { get; set; } = string.Empty;

    public string NomeCompleto =>
        string.IsNullOrWhiteSpace(Sobrenome) ? Nome : $"{Nome} {Sobrenome}".Trim();
}
