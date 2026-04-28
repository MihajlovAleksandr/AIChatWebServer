namespace AIChatWebServer.Utils.Errors
{
    public sealed class UserErrors : ErrorCode
    {
        private UserErrors(string code) : base(code) { }

        public static readonly IErrorCode UserNotFound =
            new UserErrors("USER_NOT_FOUND");

        public static readonly IErrorCode UserBanned =
            new UserErrors("USER_BANNED");

        public static readonly IErrorCode InvalidCredentials =
            new UserErrors("INVALID_CREDENTIALS");

        public static readonly IErrorCode InvalidLoginMethod =
            new UserErrors("INVALID_LOGIN_METHOD");

        public static readonly IErrorCode UserMismatch =
            new UserErrors("USER_MISMATCH");

        public static readonly IErrorCode PremiumRequired =
            new UserErrors("PREMIUM_REQUIRED");

        public static readonly IErrorCode UserNotRegistred =
            new UserErrors("USER_NOT_REGISTRED");
    }
}
