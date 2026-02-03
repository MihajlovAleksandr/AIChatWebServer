namespace AIChatWebServer.Utils.Errors
{
    public sealed class SessionErrors : ErrorCode
    {
        private SessionErrors(string code) : base(code) { }

        public static readonly IErrorCode InvalidConnection =
            new SessionErrors("SESSION_INVALID_CONNECTION");

        public static readonly IErrorCode InvalidToken =
            new SessionErrors("SESSION_INVALID_TOKEN");
    }
}
