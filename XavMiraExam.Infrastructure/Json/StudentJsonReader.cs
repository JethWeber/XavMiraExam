using System.Text.Json;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;
using XavMiraExam.Core.Services;

namespace XavMiraExam.Infrastructure.Json;

/// <summary>
/// Leitura de alunos a partir de JSON (legado / consolas de teste).
/// Desktop usa StudentSqliteService.
/// </summary>
public class StudentJsonReader : IStudentService
{
    private readonly List<Student> _students;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public StudentJsonReader(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Ficheiro de alunos não encontrado.", filePath);

        string json = File.ReadAllText(filePath);
        var items = JsonSerializer.Deserialize<List<StudentDto>>(json, JsonOptions)
                    ?? new List<StudentDto>();

        _students = new List<Student>();
        foreach (var s in items)
        {
            if (string.IsNullOrWhiteSpace(s.Codigo) && string.IsNullOrWhiteSpace(s.Nome))
                continue;

            string full = (s.Nome ?? "").Trim();
            int space = full.IndexOf(' ');
            string first = space > 0 ? full[..space] : (string.IsNullOrWhiteSpace(full) ? s.Codigo : full);
            string last = space > 0 ? full[(space + 1)..].Trim() : "Aluno";

            _students.Add(new Student
            {
                Id = Guid.NewGuid(),
                Codigo = (s.Codigo ?? "").Trim(),
                Nome = first,
                Sobrenome = last,
                SenhaHash = PasswordHasher.Hash("1234"),
                Turma = (s.Turma ?? "").Trim(),
            });
        }
    }

    public Student? Authenticate(string sobrenome, string senha)
    {
        sobrenome = (sobrenome ?? "").Trim();
        if (string.IsNullOrWhiteSpace(sobrenome) || string.IsNullOrEmpty(senha))
            return null;

        foreach (var student in _students)
        {
            if (!string.Equals(student.Sobrenome, sobrenome, StringComparison.OrdinalIgnoreCase))
                continue;
            if (PasswordHasher.Verify(senha, student.SenhaHash))
                return student;
        }

        return null;
    }

    public Student Register(string nome, string sobrenome, string senha, string? turma = null)
    {
        throw new NotSupportedException(
            "Registo via JSON não suportado. Use StudentSqliteService na app Desktop.");
    }

    public IReadOnlyList<Student> GetAll() => _students;

    private sealed class StudentDto
    {
        public string Codigo { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Turma { get; set; } = "";
    }
}
