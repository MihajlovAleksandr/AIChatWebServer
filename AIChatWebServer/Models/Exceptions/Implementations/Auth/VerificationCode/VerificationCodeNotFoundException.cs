using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.VerificationCode
{
    public sealed class VerificationCodeNotFoundException : ApiExceptionBase
    {
        public VerificationCodeNotFoundException(Guid userId, string type)
            : base(
                404,
                RegisterErrors.CodeNotFound,
                $"Verification code not found for user {userId}. Type: {type}")
        {
        }
    }
}
