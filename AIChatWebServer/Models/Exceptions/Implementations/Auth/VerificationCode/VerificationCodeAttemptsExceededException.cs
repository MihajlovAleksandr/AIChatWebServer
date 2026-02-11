using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.VerificationCode
{
    public sealed class VerificationCodeAttemptsExceededException : ApiExceptionBase
    {
        public VerificationCodeAttemptsExceededException(
            Guid userId,
            int currentAttempts,
            int maxAttempts)
            : base(
                403,
                RegisterErrors.AttemptsExceeded,
                $"Verification code attempts exceeded for user {userId}. " +
                $"Current: {currentAttempts}, Max: {maxAttempts}")
        {
        }
    }
}
