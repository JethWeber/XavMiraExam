using Microsoft.Data.Sqlite;

namespace XavMiraExam.Infrastructure.Database;

/// <summary>
/// Abre a ligação SQLite e garante o schema.
/// </summary>
public sealed class SqliteDb
{
    private readonly string _connectionString;

    public SqliteDb(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        string? dir = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();

        InitializeSchema();
    }

    public SqliteConnection OpenConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        return conn;
    }

    private void InitializeSchema()
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Students (
                Id          TEXT    NOT NULL PRIMARY KEY,
                Codigo      TEXT    NOT NULL UNIQUE COLLATE NOCASE,
                Nome        TEXT    NOT NULL,
                Sobrenome   TEXT    NOT NULL DEFAULT '',
                SenhaHash   TEXT    NOT NULL DEFAULT '',
                Turma       TEXT    NOT NULL DEFAULT '',
                CreatedAt   TEXT    NOT NULL
            );

            CREATE TABLE IF NOT EXISTS ExamResults (
                Id              TEXT    NOT NULL PRIMARY KEY,
                StudentId       TEXT    NOT NULL,
                StudentCodigo   TEXT    NOT NULL,
                StudentNome     TEXT    NOT NULL,
                ExamId          TEXT    NOT NULL,
                ExamTitle       TEXT    NOT NULL,
                TotalQuestoes   INTEGER NOT NULL,
                Acertos         INTEGER NOT NULL,
                Erros           INTEGER NOT NULL,
                NaoRespondidas  INTEGER NOT NULL,
                Percentagem     REAL    NOT NULL,
                Nota            REAL    NOT NULL,
                NotaMaxima      REAL    NOT NULL,
                DataRealizacao  TEXT    NOT NULL,
                PdfPath         TEXT    NULL
            );

            CREATE INDEX IF NOT EXISTS IX_ExamResults_StudentId
                ON ExamResults(StudentId);

            CREATE INDEX IF NOT EXISTS IX_ExamResults_StudentCodigo
                ON ExamResults(StudentCodigo);
            """;
        cmd.ExecuteNonQuery();

        EnsureColumn(conn, "Students", "Sobrenome", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(conn, "Students", "SenhaHash", "TEXT NOT NULL DEFAULT ''");
    }

    private static void EnsureColumn(SqliteConnection conn, string table, string column, string definition)
    {
        using var check = conn.CreateCommand();
        check.CommandText = $"PRAGMA table_info({table});";
        using var reader = check.ExecuteReader();
        while (reader.Read())
        {
            if (string.Equals(reader.GetString(1), column, StringComparison.OrdinalIgnoreCase))
                return;
        }
        reader.Close();

        using var alter = conn.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition};";
        alter.ExecuteNonQuery();
    }
}
