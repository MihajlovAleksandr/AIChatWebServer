using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions
{
    public sealed class AuthTokenException(IErrorCode error) : Exception
    {
        public IErrorCode Error { get; } = error;
    }
}
