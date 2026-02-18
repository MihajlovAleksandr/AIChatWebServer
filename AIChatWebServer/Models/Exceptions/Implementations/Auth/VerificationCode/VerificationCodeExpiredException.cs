using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.VerificationCode
{
    public sealed class VerificationCodeExpiredException(DateTime expiredAt, Guid userId) : ApiExceptionBase(
            400,
            RegisterErrors.CodeExpired,
            $"Verification code for user {userId} expired at {expiredAt:O}")
    {
    }
}
