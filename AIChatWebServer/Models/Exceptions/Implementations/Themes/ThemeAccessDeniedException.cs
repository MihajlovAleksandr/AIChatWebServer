using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Themes
{
    public class ThemeAccessDeniedException(Guid themeId, Guid userId) : ApiExceptionBase(403, ThemeErrors.ThemeAccessDenied, $"User {userId} attempted to modify theme {themeId} which belongs to another user")
    {
    }
}
