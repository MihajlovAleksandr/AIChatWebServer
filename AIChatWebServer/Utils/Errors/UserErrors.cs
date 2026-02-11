namespace AIChatWebServer.Utils.Errors
{
    public class UserErrors : ErrorCode
    {
        private UserErrors(string code) : base(code) { }

        public static readonly IErrorCode UserNotFound =
            new UserErrors("USER_NOT_FOUND");
        public static readonly IErrorCode UserBanned =
            new UserErrors("USER_BANNED");
    }
}
