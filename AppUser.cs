using System.Security.Cryptography;
using System.Text;

namespace AliceTrainingSystem.Data;

public static class PasswordHasher
{
    public static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }

    public static bool Verify(string plainText, string hash)
    {
        return string.Equals(Hash(plainText), hash, StringComparison.OrdinalIgnoreCase);
    }
}
