using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public sealed class InvalidLanguageCodeException(string languageCode, string? context = null) : ApiExceptionBase(400, UserErrors.InvalidLanguageCode,
              context == null
                      ? $"Invalid language code: {languageCode}"
                      : $"Invalid language code '{languageCode}' for context '{context}'")
    {
    }
}
