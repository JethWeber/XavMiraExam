namespace XavMiraExam.Core.Models;

/// <summary>
/// Representa a resposta de um aluno a uma questão específica.
/// </summary>
public class Answer
{
    /// <summary>Id da questão respondida.</summary>
    public int QuestionId { get; set; }

    /// <summary>Id do aluno que respondeu.</summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Índice da alternativa selecionada pelo aluno.
    /// Valor -1 (ou null) indica que a questão não foi respondida a tempo.
    /// </summary>
    public int? AlternativaSelecionada { get; set; }

    /// <summary>Indica se a resposta dada está correta.</summary>
    public bool Correta { get; set; }

    /// <summary>Tempo utilizado pelo aluno para responder, em segundos.</summary>
    public double TempoUtilizado { get; set; }

    /// <summary>Indica se a questão foi respondida dentro do tempo.</summary>
    public bool FoiRespondida => AlternativaSelecionada.HasValue;
}
