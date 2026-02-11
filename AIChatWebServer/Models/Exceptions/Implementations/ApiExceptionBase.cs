using AIChatWebServer.Models.Exceptions.Interfaces;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations
{
    public abstract class ApiExceptionBase : Exception, IApiException
    {
        protected ApiExceptionBase(
            int statusCode,
            IErrorCode errorCode,
            string message)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public int StatusCode { get; }

        public IErrorCode ErrorCode { get; }
    }
}
