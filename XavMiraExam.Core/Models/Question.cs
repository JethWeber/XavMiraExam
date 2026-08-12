namespace XavMiraExam.Core.Models;

/// <summary>
/// Representa uma questão de escolha múltipla dentro de uma prova.
/// </summary>
public class Question
{
    /// <summary>Identificador da questão (único dentro da prova).</summary>
    public int Id { get; set; }

    /// <summary>Enunciado da questão.</summary>
    public string Pergunta { get; set; } = string.Empty;

    /// <summary>Lista de alternativas de resposta (mínimo 2).</summary>
    public List<string> Alternativas { get; set; } = new();

    /// <summary>
    /// Índice (base 0) da alternativa correta dentro de <see cref="Alternativas"/>.
    /// </summary>
    public int Correta { get; set; }

    /// <summary>
    /// Verifica se um índice de alternativa selecionado corresponde à resposta correta.
    /// </summary>
    public bool EhCorreta(int indiceSelecionado) => indiceSelecionado == Correta;
}
