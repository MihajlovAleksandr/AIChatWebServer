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

        public static readonly IErrorCode InvalidServerType =
            new UserErrors("INVALID_SERVER_TYPE");

        public static readonly IErrorCode AuthIdentityNotFound =
            new UserErrors("AUTH_IDENTITY_NOT_FOUND");

        public static readonly IErrorCode CannotDeleteLastAuthIdentity =
            new UserErrors("CANNOT_DELETE_LAST_AUTH_IDENTITY");

        public static readonly IErrorCode InvalidLanguageCode =
            new UserErrors("INVALID_LANGUAGE_CODE");

        public static readonly IErrorCode LanguageNotFound =
            new UserErrors("LANGUAGE_NOT_FOUND");

        public static readonly IErrorCode UserAlreadyDeleted =
            new UserErrors("USER_ALREADY_DELETED");

        public static readonly IErrorCode CannotDeleteSystemUser =
            new UserErrors("CANNOT_DELETE_SYSTEM_USER");

        public static readonly IErrorCode AuthIdentityAlreadyExists =
            new UserErrors("AUTH_IDENTITY_ALREADY_EXISTS");
    }
}