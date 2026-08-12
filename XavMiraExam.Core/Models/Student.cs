namespace XavMiraExam.Core.Models;

/// <summary>
/// Representa um aluno que realiza a prova.
/// </summary>
public class Student
{
    /// <summary>Identificador interno do aluno.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Código/credencial usado pelo aluno para entrar na prova.</summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Nome completo do aluno.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Turma a que o aluno pertence.</summary>
    public string Turma { get; set; } = string.Empty;
}
