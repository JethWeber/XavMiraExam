namespace XavMiraExam.Desktop.Services;

/// <summary>
/// Caminhos da aplicação (cross-platform Linux/Windows).
/// <list type="bullet">
///   <item>BD: pasta de dados da aplicação (não visível em Documentos)</item>
///   <item>PDFs: Documentos/XavMiraExam/Results</item>
/// </list>
/// </summary>
public static class AppPaths
{
    /// <summary>
    /// Pasta de dados da aplicação (utilizador não deve mexer aqui).
    /// Linux:   ~/.local/share/XavMiraExam
    /// Windows: %LocalAppData%\XavMiraExam
    ///          (ex: C:\Users\...\AppData\Local\XavMiraExam)
    /// </summary>
    public static string LocalAppRoot
    {
        get
        {
            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrWhiteSpace(local))
            {
                // Fallback raro: ~/.local/share ou equivalente
                string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                local = Path.Combine(home, ".local", "share");
            }

            string root = Path.Combine(local, "XavMiraExam");
            Directory.CreateDirectory(root);
            return root;
        }
    }

    /// <summary>
    /// Pasta dos PDFs em Documentos (acessível ao professor/aluno).
    /// Linux:   ~/Documents/XavMiraExam
    /// Windows: C:\Users\...\Documents\XavMiraExam
    /// </summary>
    public static string DocumentsAppRoot
    {
        get
        {
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrWhiteSpace(documents))
                documents = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string root = Path.Combine(documents, "XavMiraExam");
            Directory.CreateDirectory(root);
            return root;
        }
    }

    /// <summary>
    /// Dados internos (SQLite).
    /// Linux:   ~/.local/share/XavMiraExam/Data
    /// Windows: %LocalAppData%\XavMiraExam\Data
    /// </summary>
    public static string DataFolder
    {
        get
        {
            string path = Path.Combine(LocalAppRoot, "Data");
            Directory.CreateDirectory(path);
            return path;
        }
    }

    /// <summary>Caminho completo da base de dados SQLite.</summary>
    public static string DatabasePath => Path.Combine(DataFolder, "XavMiraExam.db");

    /// <summary>
    /// PDFs por aluno — em Documentos (visível e exportável).
    /// Linux:   ~/Documents/XavMiraExam/Results
    /// Windows: Documents\XavMiraExam\Results
    /// </summary>
    public static string ResultsFolder
    {
        get
        {
            string path = Path.Combine(DocumentsAppRoot, "Results");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
