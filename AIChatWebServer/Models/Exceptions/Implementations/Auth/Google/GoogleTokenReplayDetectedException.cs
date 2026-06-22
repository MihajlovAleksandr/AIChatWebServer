using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.Google
{
    public sealed class GoogleTokenReplayDetectedException(string? email) : ApiExceptionBase(
            401,
            GoogleOAuthErrors.GoogleTokenReplayDetected,
            $"Google authentication token replay detected for email {email}.")
    {
    }
}
