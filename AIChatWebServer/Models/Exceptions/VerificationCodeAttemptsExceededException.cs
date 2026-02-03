namespace AIChatWebServer.Models.Exceptions
{
    public sealed class VerificationCodeAttemptsExceededException : Exception
    {
        public int MaxAttempts { get; }

        public VerificationCodeAttemptsExceededException(int maxAttempts)
            : base("Maximum verification attempts exceeded")
        {
            MaxAttempts = maxAttempts;
        }
    }
}
