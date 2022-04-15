using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("Enigma.String.Tests")]
namespace Enigma.String
{
    public static class EnigmaExtension
    {
        private static EnigmaOptions _options = null!;

        public static void SetOptions(EnigmaOptions options)
            => _options = options;

        public static string Encrypt(this string value)
        {
            if (string.IsNullOrEmpty(_options.SymmetricKey))
                throw new ArgumentNullException(nameof(_options.SymmetricKey));
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(nameof(value),
                    $"the value '{value}' should not be null or empty");

            var iv = new byte[16];
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_options.SymmetricKey);
            aes.IV = iv;

            var cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(value);
            }

            var array = memoryStream.ToArray();

            return Convert.ToBase64String(array);
        }

        public static string Decrypt(this string value)
        {
            if (string.IsNullOrEmpty(_options.SymmetricKey))
                throw new ArgumentNullException(nameof(_options.SymmetricKey));
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(
                    $"the value '{value}' should not be null or empty");
            if (!value.IsBase64String())
                throw new ArgumentException(nameof(value), $"this '{value}' is not a supported cipher string");

            var iv = new byte[16];
            var buffer = Convert.FromBase64String(value);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_options.SymmetricKey);
            aes.IV = iv;
            var cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream(buffer);
            using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }

        internal static bool IsBase64String(this string base64)
        {
            var buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}
