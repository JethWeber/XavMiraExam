using Microsoft.Data.Sqlite;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;

namespace XavMiraExam.Infrastructure.Database;

/// <summary>
/// Guarda e consulta resultados de prova em SQLite.
/// </summary>
public class ResultSqliteStore : IResultStore
{
    private readonly SqliteDb _db;

    public ResultSqliteStore(SqliteDb db)
    {
        _db = db;
    }

    public ExamResult Save(
        ExamResult result,
        string examTitle,
        double notaMaxima,
        string studentCodigo,
        string studentNome,
        string? pdfPath)
    {
        ArgumentNullException.ThrowIfNull(result);

        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO ExamResults (
                Id, StudentId, StudentCodigo, StudentNome,
                ExamId, ExamTitle,
                TotalQuestoes, Acertos, Erros, NaoRespondidas,
                Percentagem, Nota, NotaMaxima,
                DataRealizacao, PdfPath
            ) VALUES (
                $id, $studentId, $studentCodigo, $studentNome,
                $examId, $examTitle,
                $total, $acertos, $erros, $naoRespondidas,
                $percentagem, $nota, $notaMaxima,
                $data, $pdfPath
            );
            """;

        cmd.Parameters.AddWithValue("$id", result.Id.ToString());
        cmd.Parameters.AddWithValue("$studentId", result.StudentId.ToString());
        cmd.Parameters.AddWithValue("$studentCodigo", studentCodigo);
        cmd.Parameters.AddWithValue("$studentNome", studentNome);
        cmd.Parameters.AddWithValue("$examId", result.ExamId.ToString());
        cmd.Parameters.AddWithValue("$examTitle", examTitle);
        cmd.Parameters.AddWithValue("$total", result.TotalQuestoes);
        cmd.Parameters.AddWithValue("$acertos", result.Acertos);
        cmd.Parameters.AddWithValue("$erros", result.Erros);
        cmd.Parameters.AddWithValue("$naoRespondidas", result.NaoRespondidas);
        cmd.Parameters.AddWithValue("$percentagem", result.Percentagem);
        cmd.Parameters.AddWithValue("$nota", result.Nota);
        cmd.Parameters.AddWithValue("$notaMaxima", notaMaxima);
        cmd.Parameters.AddWithValue("$data", result.DataRealizacao.ToString("o"));
        cmd.Parameters.AddWithValue("$pdfPath", (object?)pdfPath ?? DBNull.Value);

        cmd.ExecuteNonQuery();
        return result;
    }

    public IReadOnlyList<StoredExamResult> GetByStudentCodigo(string codigo)
    {
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, StudentId, StudentCodigo, StudentNome,
                   ExamId, ExamTitle,
                   TotalQuestoes, Acertos, Erros, NaoRespondidas,
                   Percentagem, Nota, NotaMaxima, DataRealizacao, PdfPath
            FROM ExamResults
            WHERE StudentCodigo = $codigo
            ORDER BY DataRealizacao DESC;
            """;
        cmd.Parameters.AddWithValue("$codigo", codigo.Trim());

        return ReadAll(cmd);
    }

    public IReadOnlyList<StoredExamResult> GetAll()
    {
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, StudentId, StudentCodigo, StudentNome,
                   ExamId, ExamTitle,
                   TotalQuestoes, Acertos, Erros, NaoRespondidas,
                   Percentagem, Nota, NotaMaxima, DataRealizacao, PdfPath
            FROM ExamResults
            ORDER BY DataRealizacao DESC;
            """;

        return ReadAll(cmd);
    }

    private static List<StoredExamResult> ReadAll(SqliteCommand cmd)
    {
        var list = new List<StoredExamResult>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new StoredExamResult
            {
                Id = Guid.Parse(reader.GetString(0)),
                StudentId = Guid.Parse(reader.GetString(1)),
                StudentCodigo = reader.GetString(2),
                StudentNome = reader.GetString(3),
                ExamId = Guid.Parse(reader.GetString(4)),
                ExamTitle = reader.GetString(5),
                TotalQuestoes = reader.GetInt32(6),
                Acertos = reader.GetInt32(7),
                Erros = reader.GetInt32(8),
                NaoRespondidas = reader.GetInt32(9),
                Percentagem = reader.GetDouble(10),
                Nota = reader.GetDouble(11),
                NotaMaxima = reader.GetDouble(12),
                DataRealizacao = DateTime.Parse(reader.GetString(13), null,
                    System.Globalization.DateTimeStyles.RoundtripKind),
                PdfPath = reader.IsDBNull(14) ? null : reader.GetString(14),
            });
        }
        return list;
    }
}
