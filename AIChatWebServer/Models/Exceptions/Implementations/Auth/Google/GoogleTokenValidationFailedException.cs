using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.Google
{
    public sealed class GoogleTokenValidationFailedException() : ApiExceptionBase(
        401, 
        GoogleOAuthErrors.GoogleTokenValidationFailed,
        "Google authentication token validation failed.")
    {
    }
}
