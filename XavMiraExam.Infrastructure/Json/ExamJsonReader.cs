using System.Text.Json;
using System.Text.Json.Serialization;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;

namespace XavMiraExam.Infrastructure.Json;

/// <summary>
/// Implementação de <see cref="IExamJsonReader"/> usando System.Text.Json.
/// Não depende de nenhum pacote NuGet externo — apenas da biblioteca padrão do .NET.
/// </summary>
public class ExamJsonReader : IExamJsonReader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public Exam ReadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Ficheiro de prova não encontrado: {filePath}", filePath);

        string json = File.ReadAllText(filePath);
        return ReadFromString(json);
    }

    public Exam ReadFromString(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidDataException("O conteúdo do ficheiro JSON está vazio.");

        ExamDto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<ExamDto>(json, Options);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"O ficheiro JSON da prova é inválido: {ex.Message}", ex);
        }

        if (dto is null)
            throw new InvalidDataException("Não foi possível interpretar o ficheiro JSON da prova.");

        return MapToExam(dto);
    }

    private static Exam MapToExam(ExamDto dto)
    {
        var exam = new Exam
        {
            Titulo = dto.Titulo ?? string.Empty,
            TempoPorQuestao = dto.TempoPorQuestao,
            NotaMaxima = dto.NotaMaxima,
            Questoes = new List<Question>(),
        };

        if (dto.Questoes is not null)
        {
            foreach (var q in dto.Questoes)
            {
                exam.Questoes.Add(new Question
                {
                    Id = q.Id,
                    Pergunta = q.Pergunta ?? string.Empty,
                    Alternativas = q.Alternativas ?? new List<string>(),
                    Correta = q.Correta,
                });
            }
        }

        return exam;
    }

    // ---- DTOs internos: refletem exatamente a estrutura do JSON (secção 4 do plano) ----

    private class ExamDto
    {
        [JsonPropertyName("titulo")]
        public string? Titulo { get; set; }

        [JsonPropertyName("tempoPorQuestao")]
        public int TempoPorQuestao { get; set; }

        [JsonPropertyName("notaMaxima")]
        public double NotaMaxima { get; set; }

        [JsonPropertyName("questoes")]
        public List<QuestionDto>? Questoes { get; set; }
    }

    private class QuestionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("pergunta")]
        public string? Pergunta { get; set; }

        [JsonPropertyName("alternativas")]
        public List<string>? Alternativas { get; set; }

        [JsonPropertyName("correta")]
        public int Correta { get; set; }
    }
}
