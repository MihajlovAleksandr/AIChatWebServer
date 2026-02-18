namespace AIChatWebServer.Utils.Errors
{
    public sealed class GoogleOAuthErrors : ErrorCode
    {
        private GoogleOAuthErrors(string code) : base(code) { }

        public static readonly IErrorCode GoogleTokenValidationFailed =
            new GoogleOAuthErrors("AUTH_GOOGLE_TOKEN_VALIDATION_FAILED");

        public static readonly IErrorCode GoogleTokenAudienceMismatch =
            new GoogleOAuthErrors("AUTH_GOOGLE_TOKEN_AUDIENCE_MISMATCH");

        public static readonly IErrorCode GoogleTokenExpired =
            new GoogleOAuthErrors("AUTH_GOOGLE_TOKEN_EXPIRED");

        public static readonly IErrorCode GoogleTokenReplayDetected =
            new GoogleOAuthErrors("AUTH_GOOGLE_TOKEN_REPLAY_DETECTED");
    }
}
