using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Themes
{
    public sealed class ThemeAlreadyExistsException(string name, Guid userId) : ApiExceptionBase(409, ThemeErrors.ThemeAlreadyExists, $"Theme '{name}' already exists for user {userId}")
    {
    }
}