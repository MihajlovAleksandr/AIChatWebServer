namespace AIChatWebServer.Utils.Errors
{
    public sealed class OAuthErrors : ErrorCode
    {
        private OAuthErrors(string code) : base(code) { }

        public static readonly IErrorCode ContextMissing =
            new OAuthErrors("OAUTH_CONTEXT_MISSING");

        public static readonly IErrorCode TokenInvalid =
            new OAuthErrors("OAUTH_TOKEN_INVALID");

        public static readonly IErrorCode UserUnauthorized =
            new OAuthErrors("OAUTH_USER_UNAUTHORIZED");

        public static readonly IErrorCode LoginFailed =
            new OAuthErrors("OAUTH_LOGIN_FAILED");

        public static readonly IErrorCode RegisterFailed =
            new OAuthErrors("OAUTH_REGISTER_FAILED");
    }
}
