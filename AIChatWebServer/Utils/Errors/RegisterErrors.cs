namespace AIChatWebServer.Utils.Errors
{
    public sealed class RegisterErrors : ErrorCode
    {
        private RegisterErrors(string code) : base(code) { }

        public static readonly IErrorCode UserAlreadyExists =
            new RegisterErrors("REGISTER_USER_ALREADY_EXISTS");

        public static readonly IErrorCode InvalidContext =
            new RegisterErrors("REGISTER_INVALID_CONTEXT");

        public static readonly IErrorCode InvalidStep =
            new RegisterErrors("REGISTER_INVALID_STEP");

        public static readonly IErrorCode InvalidCode =
            new RegisterErrors("REGISTER_INVALID_CODE");

        public static readonly IErrorCode CodeExpired =
            new RegisterErrors("REGISTER_CODE_EXPIRED");

        public static readonly IErrorCode CodeNotFound =
            new RegisterErrors("REGISTER_CODE_NOT_FOUND");

        public static readonly IErrorCode AttemptsExceeded =
            new RegisterErrors("REGISTER_ATTEMPTS_EXCEEDED");
    }
}
