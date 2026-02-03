namespace AIChatWebServer.Utils.Errors
{
    public sealed class LoginErrors : ErrorCode
    {
        private LoginErrors(string code) : base(code) { }

        public static readonly IErrorCode IdentifierRequired =
            new LoginErrors("LOGIN_IDENTIFIER_REQUIRED");

        public static readonly IErrorCode SecretRequired =
            new LoginErrors("LOGIN_SECRET_REQUIRED");

        public static readonly IErrorCode ProviderRequired =
            new LoginErrors("LOGIN_PROVIDER_REQUIRED");

        public static readonly IErrorCode InvalidCredentials =
            new LoginErrors("LOGIN_INVALID_CREDENTIALS");

        public static readonly IErrorCode RegistrationNotCompleted =
            new LoginErrors("LOGIN_REGISTRATION_NOT_COMPLETED");

        public static readonly IErrorCode InvalidWorkToken =
            new LoginErrors("LOGIN_INVALID_WORK_TOKEN");

        public static readonly IErrorCode InvalidConnection =
            new LoginErrors("LOGIN_INVALID_CONNECTION");
    }
}
