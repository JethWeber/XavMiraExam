using System.Security.Cryptography;
using System.Text;

namespace XavMiraExam.Core.Services;

/// <summary>
/// Hash de senhas com SHA-256 + salt da aplicação (MVP offline).
/// </summary>
public static class PasswordHasher
{
    private const string AppSalt = "XavMiraExam.V0.1.Salt!";

    public static string Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        var bytes = Encoding.UTF8.GetBytes(AppSalt + password);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            return false;
        return string.Equals(Hash(password), storedHash, StringComparison.OrdinalIgnoreCase);
    }
}
