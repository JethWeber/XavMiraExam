namespace XavMiraExam.Core.Models;

/// <summary>
/// Representa o resultado final de uma prova realizada por um aluno.
/// </summary>
public class ExamResult
{
    /// <summary>Identificador do resultado.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Id do aluno avaliado.</summary>
    public Guid StudentId { get; set; }

    /// <summary>Id da prova avaliada.</summary>
    public Guid ExamId { get; set; }

    /// <summary>Número total de questões da prova.</summary>
    public int TotalQuestoes { get; set; }

    /// <summary>Número de respostas corretas.</summary>
    public int Acertos { get; set; }

    /// <summary>Número de respostas incorretas.</summary>
    public int Erros { get; set; }

    /// <summary>Número de questões não respondidas (tempo esgotado).</summary>
    public int NaoRespondidas { get; set; }

    /// <summary>Percentagem de aproveitamento (0-100).</summary>
    public double Percentagem { get; set; }

    /// <summary>Nota final, na escala definida pela prova (ex: 0-20).</summary>
    public double Nota { get; set; }

    /// <summary>Data e hora em que a prova foi realizada.</summary>
    public DateTime DataRealizacao { get; set; } = DateTime.Now;

    /// <summary>Lista de questões respondidas incorretamente, para o relatório detalhado.</summary>
    public List<Answer> RespostasErradas { get; set; } = new();
}
