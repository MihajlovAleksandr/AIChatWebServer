using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Interfaces;
using Google.Apis.Auth;

namespace AIChatWebServer.Services.Implementations
{
    public class GoogleAuthValidator(
        IConfiguration configuration,
        ILogger<GoogleAuthValidator> logger,
        ITokenReplayGuard replayGuard) : IOAuthValidator
    {
        private readonly ILogger<GoogleAuthValidator> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly string _googleClientId = configuration["Google:ClientId"]
                ?? throw new InvalidOperationException("Google ClientId is not configured.");
        private readonly ITokenReplayGuard _replayGuard = replayGuard ?? throw new ArgumentNullException(nameof(replayGuard));

        public async Task<OAuthUser?> ValidateAsync(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
            {
                _logger.LogWarning("Received empty or null auth token.");
                return null;
            }

            GoogleJsonWebSignature.Payload? payload = await ValidateToken(authToken);

            if (payload == null)
            {
                _logger.LogWarning("Token validation failed: payload is null.");
                return null;
            }

            if ((string)payload.Audience != _googleClientId)
            {
                _logger.LogWarning("Token audience mismatch. Expected: {ClientId}, Actual: {Audience}",
                    _googleClientId, payload.Audience);
                return null;
            }

            var expSeconds = payload.ExpirationTimeSeconds ?? 0;
            if (expSeconds <= 0)
            {
                _logger.LogWarning("Token expiration time is missing or invalid.");
                return null;
            }

            var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;

            var isUsed = await _replayGuard.TryMarkAsUsedAsync(authToken, expiresAtUtc);
            if (isUsed)
            {
                _logger.LogWarning("Replay detected: Google token already used. Email: {Email}", payload.Email);
                return null;
            }

            _logger.LogInformation("Token successfully validated for user {UserId} with email {Email}.",
                payload.Subject, payload.Email);

            return new OAuthUser(payload.Subject, payload.Email);
        }

        private async Task<GoogleJsonWebSignature.Payload?> ValidateToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    ForceGoogleCertRefresh = true,
                    ExpirationTimeClockTolerance = TimeSpan.FromMinutes(5)
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                _logger.LogInformation("Google token successfully parsed for user {UserId}.", payload.Subject);
                return payload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google auth token.");
                return null;
            }
        }
    }
}
