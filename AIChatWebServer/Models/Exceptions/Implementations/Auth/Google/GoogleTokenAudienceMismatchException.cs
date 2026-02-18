using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.Google
{
    public sealed class GoogleTokenAudienceMismatchException(string expected, string actual) : ApiExceptionBase(
            401,
            GoogleOAuthErrors.GoogleTokenAudienceMismatch,
            $"Google token audience mismatch. Expected: {expected}, Actual: {actual}")
    {
    }
}
