using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Themes
{
    public sealed class SelectedThemeNotFoundException(Guid connectionId) : ApiExceptionBase(404, ThemeErrors.SelectedThemeNotFound, $"Selected theme not found for connection {connectionId}")
    {
    }
}