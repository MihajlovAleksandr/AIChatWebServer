namespace AIChatWebServer.Utils.Errors
{
    public sealed class CodeErrors : ErrorCode
    {
        private CodeErrors(string code) : base(code) { }

        public static readonly IErrorCode ContextInvalid =
            new CodeErrors("CODE_CONTEXT_INVALID");

        public static readonly IErrorCode DeviceMissing =
            new CodeErrors("CODE_DEVICE_MISSING");

        public static readonly IErrorCode ConnectionInvalid =
            new CodeErrors("CODE_CONNECTION_INVALID");

        public static readonly IErrorCode InvalidCode =
            new CodeErrors("CODE_INVALID_CODE");

        public static readonly IErrorCode CodeExpired =
            new CodeErrors("CODE_EXPIRED");

        public static readonly IErrorCode CodeNotFound =
            new CodeErrors("CODE_NOT_FOUND");

        public static readonly IErrorCode AttemptsExceeded =
            new CodeErrors("CODE_ATTEMPTS_EXCEEDED");

        public static readonly IErrorCode VerificationFailed =
            new CodeErrors("CODE_VERIFICATION_FAILED");

        public static readonly IErrorCode GenerateFailed =
            new CodeErrors("CODE_GENERATE_FAILED");
    }
}
