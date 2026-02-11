using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth
{
    public class UserBannedException(UserBan userBan) : 
        ApiExceptionBase(403, UserErrors.UserBanned, $"User {{{userBan.UserId}}} is banned until {userBan.BannedUntil}. Reason: {userBan.Reason}")
    {
        public UserBan UserBan => userBan;
    }
}
