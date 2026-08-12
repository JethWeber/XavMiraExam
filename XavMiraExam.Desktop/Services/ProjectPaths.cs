namespace XavMiraExam.Desktop.Services;

/// <summary>
/// Localiza pastas de dados (Exams, Students, etc.) subindo a partir do diretório da aplicação.
/// </summary>
public static class ProjectPaths
{
    public static string FindFolder(string folderName)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            string candidate = Path.Combine(dir.FullName, folderName);
            if (Directory.Exists(candidate))
                return candidate;

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException($"Pasta '{folderName}' não encontrada.");
    }

    public static string FindFile(string folderName, string fileName)
    {
        return Path.Combine(FindFolder(folderName), fileName);
    }
}
