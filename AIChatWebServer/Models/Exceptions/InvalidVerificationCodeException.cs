namespace AIChatWebServer.Models.Exceptions
{
    public sealed class InvalidVerificationCodeException : Exception
    {
        public InvalidVerificationCodeException()
            : base("Invalid verification code")
        {
        }
    }
}
