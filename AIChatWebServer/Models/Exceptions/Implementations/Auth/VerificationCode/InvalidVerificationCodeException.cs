using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.VerificationCode
{
    public sealed class InvalidVerificationCodeException(Guid userId) : ApiExceptionBase(
            400,
            RegisterErrors.InvalidCode,
            $"Invalid verification code provided by user {userId}")
    {
    }
}
