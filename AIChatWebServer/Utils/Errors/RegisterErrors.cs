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

        public static readonly IErrorCode EmailNotVerified =
            new RegisterErrors("REGISTER_EMAIL_NOT_VERIFIED");

        public static readonly IErrorCode VerificationCodeRequired =
            new RegisterErrors("REGISTER_VERIFICATION_CODE_REQUIRED");

        public static readonly IErrorCode InvalidCode =
            new RegisterErrors("REGISTER_INVALID_CODE");

        public static readonly IErrorCode CodeExpired =
            new RegisterErrors("REGISTER_CODE_EXPIRED");

        public static readonly IErrorCode CodeNotFound =
            new RegisterErrors("REGISTER_CODE_NOT_FOUND");

        public static readonly IErrorCode AttemptsExceeded =
            new RegisterErrors("REGISTER_ATTEMPTS_EXCEEDED");

        public static readonly IErrorCode UserDataSaveFailed =
            new RegisterErrors("REGISTER_USERDATA_SAVE_FAILED");

        public static readonly IErrorCode PreferenceSaveFailed =
            new RegisterErrors("REGISTER_PREFERENCE_SAVE_FAILED");
    }
}
