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
                using (var sha256 = SHA256.Create())
                {
                    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                    var builder = new StringBuilder();
                    foreach (byte b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }

                    string hash = builder.ToString();
                    _logger.LogInformation("Data hashed successfully. Hash length: {Length}", hash.Length);

                    return hash;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while hashing data");
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
                bool isMatch = string.Equals(computedHash, hashedData, StringComparison.OrdinalIgnoreCase);

                if (isMatch)
                {
                    _logger.LogInformation("Data verification succeeded.");
                }
                else
                {
                    _logger.LogWarning("Data verification failed.");
                }

                return isMatch;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while verifying data.");
                throw;
            }
        }
    }
}
