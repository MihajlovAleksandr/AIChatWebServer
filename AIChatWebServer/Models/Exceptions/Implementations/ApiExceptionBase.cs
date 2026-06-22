using AIChatWebServer.Models.Exceptions.Interfaces;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations
{
    public abstract class ApiExceptionBase(
        int statusCode,
        IErrorCode errorCode,
        string message) : Exception(message), IApiException
    {
        public int StatusCode { get; } = statusCode;

        public IErrorCode ErrorCode { get; } = errorCode;
    }
}
