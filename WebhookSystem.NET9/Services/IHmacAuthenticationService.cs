using System.Security.Cryptography;
using System.Text;

namespace WebhookSystem.NET9.Services
{
    public interface IHmacAuthenticationService
    {
        string GenerateSignature(string payload, string secret, string algorithm = "sha256");
        bool ValidateSignature(string payload, string signature, string secret, string algorithm = "sha256");
        string GenerateTimestamp();
        bool ValidateTimestamp(string timestamp, TimeSpan tolerance = default);
    }

    public class HmacAuthenticationService : IHmacAuthenticationService
    {
        private readonly ILogger<HmacAuthenticationService> _logger;

        public HmacAuthenticationService(ILogger<HmacAuthenticationService> logger)
        {
            _logger = logger;
        }

        public string GenerateSignature(string payload, string secret, string algorithm = "sha256")
        {
            try
            {
                var keyBytes = Convert.FromBase64String(secret);
                var payloadBytes = Encoding.UTF8.GetBytes(payload);
                using var hmac = algorithm.ToLowerInvariant() switch
                {
                    "sha1" => HMAC.Create("HMACSHA1"),
                    "sha256" => HMAC.Create("HMACSHA256"),
                    "sha512" => HMAC.Create("HMACSHA512"),
                    _ => throw new ArgumentException($"Unsupported algorithm: {algorithm}")
                };
                hmac!.Key = keyBytes;
                var hashBytes = hmac.ComputeHash(payloadBytes);

                return Convert.ToHexString(hashBytes).ToLowerInvariant();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating HMAC signature");
                throw;
            }
        }

        public bool ValidateSignature(string payload, string signature, string secret, string algorithm = "sha256")
        {
            try
            {
                // Remove algorithm prefix if present (e.g., "sha256=" from GitHub)
                var cleanSignature = signature.Contains('=')
                    ? signature.Split('=')[1]
                    : signature;
                var expectedSignature = GenerateSignature(payload, secret, algorithm);

                // Use time-constant comparison to prevent timing attacks
                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(cleanSignature),
                    Encoding.UTF8.GetBytes(expectedSignature));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating HMAC signature");
                return false;
            }
        }

        public string GenerateTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        public bool ValidateTimestamp(string timestamp, TimeSpan tolerance = default)
        {
            if (tolerance == default)
                tolerance = TimeSpan.FromMinutes(5); // Default 5-minute tolerance
            try
            {
                if (!long.TryParse(timestamp, out var unixTimestamp))
                    return false;
                var providedTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
                var now = DateTimeOffset.UtcNow;

                return Math.Abs((now - providedTime).TotalMilliseconds) <= tolerance.TotalMilliseconds;
            }
            catch
            {
                return false;
            }
        }
    }
}
