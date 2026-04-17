// Services/EncryptionService.cs
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ChildernOfTheBible.Services
{
    public class EncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IConfiguration config)
        {
            var raw = config["Security:EncryptionKey"]
                      ?? throw new InvalidOperationException("Encryption key missing");
            // Derive a 256-bit key from the config value
            _key = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        }

        public string Encrypt(string? plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();
            using var enc = aes.CreateEncryptor();
            var plain = Encoding.UTF8.GetBytes(plainText);
            var cipher = enc.TransformFinalBlock(plain, 0, plain.Length);
            // Prepend IV so we can decrypt later
            var result = new byte[aes.IV.Length + cipher.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipher, 0, result, aes.IV.Length, cipher.Length);
            return Convert.ToBase64String(result);
        }

        public string Decrypt(string? cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            var data = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = _key;
            var iv = data[..16];
            var cipher = data[16..];
            aes.IV = iv;
            using var dec = aes.CreateDecryptor();
            var plain = dec.TransformFinalBlock(cipher, 0, cipher.Length);
            return Encoding.UTF8.GetString(plain);
        }
    }
}
