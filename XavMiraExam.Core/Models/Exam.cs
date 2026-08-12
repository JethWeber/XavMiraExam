namespace XavMiraExam.Core.Models;

/// <summary>
/// Representa uma prova completa, carregada a partir de um ficheiro JSON.
/// </summary>
public class Exam
{
    /// <summary>Identificador interno da prova (gerado localmente, não vem do JSON).</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Título da prova.</summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>Tempo, em segundos, atribuído a cada questão.</summary>
    public int TempoPorQuestao { get; set; }

    /// <summary>Nota máxima possível na prova (ex: 20 valores).</summary>
    public double NotaMaxima { get; set; }

    /// <summary>Lista de questões da prova.</summary>
    public List<Question> Questoes { get; set; } = new();
}
