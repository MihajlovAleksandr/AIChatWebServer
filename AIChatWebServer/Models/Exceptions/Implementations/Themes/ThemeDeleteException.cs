using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Themes
{
    public sealed class ThemeDeleteException : ApiExceptionBase
    {
        public ThemeDeleteException(Guid themeId) : base(400, ThemeErrors.ThemeDeleteFailed, $"Failed to delete theme {themeId}")
        { }

        public ThemeDeleteException(Guid themeId, string reason) : base(400, ThemeErrors.ThemeDeleteFailed, $"Failed to delete theme {themeId}: {reason}")
        { }
    }
}