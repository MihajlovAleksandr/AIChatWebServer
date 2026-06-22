namespace AIChatWebServer.Utils.Errors
{
    public sealed class CodeErrors : ErrorCode
    {
        private CodeErrors(string code) : base(code) { }

        public static readonly IErrorCode ContextInvalid =
            new CodeErrors("CODE_CONTEXT_INVALID");
    }
}
