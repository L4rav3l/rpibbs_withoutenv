using System.Security.Cryptography;
using System.Text;

namespace RPIBBS.Core;

public class Encrypting
{
    public string Sha512Generate(string text)
    {
        var sha512 = SHA512.Create();
        byte[] hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(text));

        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}