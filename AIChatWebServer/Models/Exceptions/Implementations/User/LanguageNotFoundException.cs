using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public sealed class LanguageNotFoundException(Guid userId, LanguageContext context) : ApiExceptionBase(404, UserErrors.LanguageNotFound, $"Language for user {userId} with context '{context}' was not found")
    {
    }
}
