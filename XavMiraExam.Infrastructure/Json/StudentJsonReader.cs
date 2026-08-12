using System.Text.Json;
using System.Text.Json.Serialization;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;

namespace XavMiraExam.Infrastructure.Json;

/// <summary>
/// Carrega alunos a partir de um ficheiro JSON local (sem base de dados).
/// </summary>
public class StudentJsonReader : IStudentService
{
    private readonly Dictionary<string, Student> _studentsByCodigo;

    public StudentJsonReader(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Ficheiro de alunos não encontrado: {filePath}", filePath);

        _studentsByCodigo = LoadStudents(filePath);
    }

    public Student? FindByCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return null;

        string key = codigo.Trim();
        return _studentsByCodigo.TryGetValue(key, out Student? student) ? student : null;
    }

    private static Dictionary<string, Student> LoadStudents(string filePath)
    {
        string json = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        List<StudentDto>? dtos;
        try
        {
            dtos = JsonSerializer.Deserialize<List<StudentDto>>(json, options);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"O ficheiro JSON de alunos é inválido: {ex.Message}", ex);
        }

        if (dtos is null || dtos.Count == 0)
            throw new InvalidDataException("O ficheiro de alunos não contém registos.");

        var result = new Dictionary<string, Student>(StringComparer.OrdinalIgnoreCase);

        foreach (var dto in dtos)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                continue;

            string codigo = dto.Codigo.Trim();
            result[codigo] = new Student
            {
                Codigo = codigo,
                Nome = dto.Nome?.Trim() ?? string.Empty,
                Turma = dto.Turma?.Trim() ?? string.Empty,
            };
        }

        if (result.Count == 0)
            throw new InvalidDataException("Nenhum aluno válido foi encontrado no ficheiro.");

        return result;
    }

    private class StudentDto
    {
        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("turma")]
        public string? Turma { get; set; }
    }
}
