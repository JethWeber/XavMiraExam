using System.Security.Cryptography;
using System.Text;

namespace XavMiraExam.Core.Services;

/// <summary>
/// Credenciais embutidas do professor (V0.1 — sem registo).
/// Utilizador: Santareno
/// A senha nunca é comparada em texto claro: apenas o hash SHA-256 é verificado.
/// </summary>
public static class ProfessorAuth
{
    public const string NomeCompleto = "Joaquim Santareno";
    public const string Username = "Santareno";

    // SHA-256(salt + "Jeth@2026_hidden") em hex — gerado com o mesmo PasswordHasher.
    // Assim a senha em claro não aparece como constante pública comparável.
    private static readonly string PasswordHash =
        PasswordHasher.Hash("Jeth@2026_hidden");

    /// <summary>
    /// Valida utilizador + senha do professor.
    /// </summary>
    public static bool Validate(string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            return false;

        if (!string.Equals(username.Trim(), Username, StringComparison.OrdinalIgnoreCase))
            return false;

        return PasswordHasher.Verify(password, PasswordHash);
    }
}
