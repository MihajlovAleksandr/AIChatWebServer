using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Message
{
    public class InvalidQuoteRangeException(int? startIndex, int? endIndex)
        : ApiExceptionBase(
            400,
            MessageErrors.InvalidQuoteRange,
            $"Quote start index ({startIndex}) cannot be greater than end index ({endIndex})")
    {
    }
}