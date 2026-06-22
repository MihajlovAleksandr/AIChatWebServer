using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Themes
{
    public sealed class ThemeNotFoundException : ApiExceptionBase
    {
        public ThemeNotFoundException(Guid themeId) : base(404, ThemeErrors.ThemeNotFound, $"Theme {themeId} was not found")
        { }

        public ThemeNotFoundException(string name, Guid userId) : base(404, ThemeErrors.ThemeNotFound, $"Theme '{name}' was not found for user {userId}")
        { }
    }
}