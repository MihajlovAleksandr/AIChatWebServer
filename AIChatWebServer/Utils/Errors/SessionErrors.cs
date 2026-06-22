namespace AIChatWebServer.Utils.Errors
{
    public sealed class SessionErrors : ErrorCode
    {
        private SessionErrors(string code) : base(code) { }

        public static readonly IErrorCode InvalidConnection =
            new SessionErrors("SESSION_INVALID_CONNECTION");

        public static readonly IErrorCode InvalidToken =
            new SessionErrors("SESSION_INVALID_TOKEN");

        public static readonly IErrorCode ConnectionNotFound =
            new SessionErrors("SESSION_CONNECTION_NOT_FOUND");

        public static readonly IErrorCode ConnectionForbidden =
            new SessionErrors("SESSION_CONNECTION_FORBIDDEN");
    }
}
