using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Context
{
    public sealed class LanguageMissingException() : ApiExceptionBase(400, CommonErrors.LanguageMissing, "Language code is missing")
    {
    }
}
