using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.Google
{
    public sealed class GoogleTokenExpiredException : ApiExceptionBase
    {
        public GoogleTokenExpiredException()
            : base(
                401,
                GoogleOAuthErrors.GoogleTokenExpired,
                "Google authentication token has expired or contains invalid expiration time.")
        {
        }
    }
}
