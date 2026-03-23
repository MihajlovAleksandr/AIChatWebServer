using AIChatWebServer.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class Hasher(ILogger<Hasher> logger) : IHasher
    {
        private readonly ILogger<Hasher> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        public string Hash(string data)
        {
            ArgumentNullException.ThrowIfNull(data);

            try
            {
                using var sha256 = SHA256.Create();

                var bytes = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(data));

                return ToHex(bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while hashing data");
                throw;
            }
        }

        public async Task<string> HashAsync(
            Stream stream,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(stream);

            try
            {
                using var sha256 = SHA256.Create();

                byte[] hash =
                    await sha256.ComputeHashAsync(stream, ct);

                string result = ToHex(hash);

                _logger.LogInformation(
                    "Stream hashed successfully. Length: {Length}",
                    result.Length);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while hashing stream");
                throw;
            }
        }

        public bool Verify(string data, string hashedData)
        {
            ArgumentNullException.ThrowIfNull(data);
            ArgumentNullException.ThrowIfNull(hashedData);

            try
            {
                string computedHash = Hash(data);

                return string.Equals(
                    computedHash,
                    hashedData,
                    StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while verifying data.");
                throw;
            }
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}