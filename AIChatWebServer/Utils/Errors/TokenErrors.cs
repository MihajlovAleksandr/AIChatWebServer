namespace AIChatWebServer.Utils.Errors
{
    public sealed class TokenErrors : ErrorCode
    {
        private TokenErrors(string code) : base(code) { }


        public static readonly IErrorCode InvalidToken =
            new TokenErrors("TOKEN_INVALID");

        public static readonly IErrorCode InvalidType =
            new TokenErrors("TOKEN_INVALID_TYPE");

        public static readonly IErrorCode Unauthorized =
            new TokenErrors("TOKEN_UNAUTHORIZED");
    }
}
