using AIChatWebServer.Models.Exceptions.Interfaces;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth
{
    public sealed class AuthTokenException(IErrorCode errorCode) : ApiExceptionBase(401, errorCode, $"AuthTokenException: {errorCode.Code}"), IApiException
    {
    }
}
