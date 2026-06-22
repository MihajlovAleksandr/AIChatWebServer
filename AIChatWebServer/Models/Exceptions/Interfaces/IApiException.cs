using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Interfaces
{
    public interface IApiException
    {
        int StatusCode { get; }
        IErrorCode ErrorCode { get; }
    }
}
