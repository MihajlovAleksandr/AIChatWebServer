using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Interfaces;
using Google.Apis.Auth;
using AIChatWebServer.Models.Exceptions.Implementations.Auth.Google;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class GoogleAuthValidator(
        IConfiguration configuration,
        ILogger<GoogleAuthValidator> logger,
        ITokenReplayGuard replayGuard) : IOAuthValidator
    {
        private readonly ILogger<GoogleAuthValidator> _logger = logger;
        private readonly string _googleClientId = configuration["Google:ClientId"]
                ?? throw new InvalidOperationException("Google ClientId is not configured.");
        private readonly ITokenReplayGuard _replayGuard = replayGuard;

        public async Task<OAuthUser> ValidateAsync(string authToken)
        {
            var payload = await ValidateToken(authToken);

            if (!string.Equals((string)payload.Audience, _googleClientId, StringComparison.Ordinal))
            {
                _logger.LogWarning(
                    "Token audience mismatch. Expected: {ClientId}, Actual: {Audience}",
                    _googleClientId,
                    payload.Audience);

                throw new GoogleTokenAudienceMismatchException(_googleClientId, (string)payload.Audience);
            }

            if (!payload.ExpirationTimeSeconds.HasValue || payload.ExpirationTimeSeconds.Value <= 0)
            {
                _logger.LogWarning("Token expiration time is missing or invalid.");
                throw new GoogleTokenExpiredException();
            }

            var expiresAtUtc =
                DateTimeOffset.FromUnixTimeSeconds(payload.ExpirationTimeSeconds.Value).UtcDateTime;

            var isUsed = await _replayGuard.TryMarkAsUsedAsync(authToken, expiresAtUtc);

            if (isUsed)
            {
                _logger.LogWarning(
                    "Replay detected: Google token already used. Email: {Email}",
                    payload.Email);

                throw new GoogleTokenReplayDetectedException(payload.Email);
            }

            _logger.LogInformation(
                "Token successfully validated for user {UserId} with email {Email}.",
                payload.Subject,
                payload.Email);

            return new OAuthUser(payload.Subject, payload.Email);
        }

        private async Task<GoogleJsonWebSignature.Payload> ValidateToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    ForceGoogleCertRefresh = true,
                    ExpirationTimeClockTolerance = TimeSpan.FromMinutes(5)
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                _logger.LogInformation(
                    "Google token successfully parsed for user {UserId}.",
                    payload.Subject);

                return payload ?? throw new GoogleTokenValidationFailedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google auth token.");
                throw new GoogleTokenValidationFailedException();
            }
        }
    }
}
