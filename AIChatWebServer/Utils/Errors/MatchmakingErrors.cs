namespace AIChatWebServer.Utils.Errors
{
    public class MatchmakingErrors : ErrorCode
    {
        private MatchmakingErrors(string code) : base(code)
        {
        }

        public static readonly IErrorCode UserAlreadySearchingChat =
            new MatchmakingErrors("USER_ALREADY_SEARCHING_CHAT");

        public static readonly IErrorCode UserNotSearchingChat =
            new MatchmakingErrors("USER_NOT_SEARCHING_CHAT");

        public static readonly IErrorCode InvalidSlots =
            new MatchmakingErrors("INVALID_SLOTS");
    }
}