using System.Security.Cryptography;

namespace sttbproject.Commons.Services;

public class PasswordHashService : IPasswordHashService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    public string HashPassword(string password)
    {
        // Use BCrypt for new passwords
        return BCrypt.Net.BCrypt.HashPassword(password, 11);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        // Check if it's a BCrypt hash (starts with $2a$ or $2b$)
        if (passwordHash.StartsWith("$2a$") || passwordHash.StartsWith("$2b$"))
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                return false;
            }
        }

        // Otherwise, use PBKDF2 for backward compatibility
        try
        {
            var hashBytes = Convert.FromBase64String(passwordHash);
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using var algorithm = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA512);

            var hash = algorithm.GetBytes(HashSize);

            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}

