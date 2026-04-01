using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Message
{
    public class QuoteIndexOutOfRangeException(int? startIndex, int? endIndex, int messageLength)
        : ApiExceptionBase(
            400,
            MessageErrors.QuoteIndexOutOfRange,
            $"Quote indices (start: {startIndex}, end: {endIndex}) are out of message bounds (length: {messageLength})")
    {
    }
}