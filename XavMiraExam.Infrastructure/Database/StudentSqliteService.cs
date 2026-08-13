using Microsoft.Data.Sqlite;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;
using XavMiraExam.Core.Services;

namespace XavMiraExam.Infrastructure.Database;

/// <summary>
/// Alunos em SQLite. Login do aluno: Sobrenome + Senha.
/// </summary>
public class StudentSqliteService : IStudentService
{
    private readonly SqliteDb _db;

    public StudentSqliteService(SqliteDb db)
    {
        _db = db;
    }

    public Student? Authenticate(string sobrenome, string senha)
    {
        sobrenome = (sobrenome ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(sobrenome) || string.IsNullOrEmpty(senha))
            return null;

        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, Codigo, Nome, Sobrenome, SenhaHash, Turma
            FROM Students
            WHERE Sobrenome = $sobrenome COLLATE NOCASE;
            """;
        cmd.Parameters.AddWithValue("$sobrenome", sobrenome);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var student = MapStudent(reader);
            if (PasswordHasher.Verify(senha, student.SenhaHash))
                return student;
        }

        return null;
    }

    public Student Register(string nome, string sobrenome, string senha, string? turma = null)
    {
        nome = (nome ?? string.Empty).Trim();
        sobrenome = (sobrenome ?? string.Empty).Trim();
        turma = (turma ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(nome))
            throw new InvalidOperationException("O nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(sobrenome))
            throw new InvalidOperationException("O sobrenome é obrigatório.");
        if (string.IsNullOrEmpty(senha) || senha.Length < 4)
            throw new InvalidOperationException("A senha deve ter pelo menos 4 caracteres.");

        using (var connCheck = _db.OpenConnection())
        using (var cmdCheck = connCheck.CreateCommand())
        {
            cmdCheck.CommandText = """
                SELECT COUNT(1) FROM Students
                WHERE Nome = $nome COLLATE NOCASE
                  AND Sobrenome = $sobrenome COLLATE NOCASE;
                """;
            cmdCheck.Parameters.AddWithValue("$nome", nome);
            cmdCheck.Parameters.AddWithValue("$sobrenome", sobrenome);
            long exists = (long)(cmdCheck.ExecuteScalar() ?? 0L);
            if (exists > 0)
                throw new InvalidOperationException(
                    $"Já existe uma conta para \"{nome} {sobrenome}\".");
        }

        var student = new Student
        {
            Id = Guid.NewGuid(),
            Codigo = GenerateCodigo(nome, sobrenome),
            Nome = nome,
            Sobrenome = sobrenome,
            SenhaHash = PasswordHasher.Hash(senha),
            Turma = string.IsNullOrWhiteSpace(turma) ? "Sem turma" : turma,
        };

        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Students (Id, Codigo, Nome, Sobrenome, SenhaHash, Turma, CreatedAt)
            VALUES ($id, $codigo, $nome, $sobrenome, $senhaHash, $turma, $createdAt);
            """;
        cmd.Parameters.AddWithValue("$id", student.Id.ToString());
        cmd.Parameters.AddWithValue("$codigo", student.Codigo);
        cmd.Parameters.AddWithValue("$nome", student.Nome);
        cmd.Parameters.AddWithValue("$sobrenome", student.Sobrenome);
        cmd.Parameters.AddWithValue("$senhaHash", student.SenhaHash);
        cmd.Parameters.AddWithValue("$turma", student.Turma);
        cmd.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("o"));
        cmd.ExecuteNonQuery();

        return student;
    }

    public IReadOnlyList<Student> GetAll()
    {
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, Codigo, Nome, Sobrenome, SenhaHash, Turma
            FROM Students
            ORDER BY Nome COLLATE NOCASE, Sobrenome COLLATE NOCASE;
            """;

        var list = new List<Student>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapStudent(reader));
        return list;
    }

    /// <summary>
    /// Importa alunos legados de students.json se a tabela estiver vazia.
    /// Senha padrão dos seeds: "1234".
    /// </summary>
    public int SeedFromJsonIfEmpty(string studentsJsonPath)
    {
        if (!File.Exists(studentsJsonPath))
            return 0;

        if (GetAll().Count > 0)
            return 0;

        string json = File.ReadAllText(studentsJsonPath);
        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        List<StudentSeedDto>? seeds;
        try
        {
            seeds = System.Text.Json.JsonSerializer.Deserialize<List<StudentSeedDto>>(json, options);
        }
        catch
        {
            return 0;
        }

        if (seeds is null || seeds.Count == 0)
            return 0;

        int count = 0;
        foreach (var s in seeds)
        {
            if (string.IsNullOrWhiteSpace(s.Nome))
                continue;

            string full = s.Nome.Trim();
            int space = full.IndexOf(' ');
            string first = space > 0 ? full[..space] : full;
            string last = space > 0 ? full[(space + 1)..].Trim() : "Aluno";

            try
            {
                Register(first, last, "1234", s.Turma ?? "Turma A");
                count++;
            }
            catch (InvalidOperationException)
            {
            }
        }

        return count;
    }

    private static string GenerateCodigo(string nome, string sobrenome)
    {
        string baseCode = $"{nome.FirstOrDefault()}{sobrenome}".ToUpperInvariant();
        baseCode = new string(baseCode.Where(char.IsLetterOrDigit).ToArray());
        if (baseCode.Length > 8) baseCode = baseCode[..8];
        return baseCode + Random.Shared.Next(100, 999);
    }

    private static Student MapStudent(SqliteDataReader reader) => new()
    {
        Id = Guid.Parse(reader.GetString(0)),
        Codigo = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
        Nome = reader.GetString(2),
        Sobrenome = reader.FieldCount > 3 && !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty,
        SenhaHash = reader.FieldCount > 4 && !reader.IsDBNull(4) ? reader.GetString(4) : string.Empty,
        Turma = reader.FieldCount > 5 && !reader.IsDBNull(5) ? reader.GetString(5) : string.Empty,
    };

    private sealed class StudentSeedDto
    {
        public string Codigo { get; set; } = "";
        public string Nome { get; set; } = "";
        public string? Turma { get; set; }
    }
}
