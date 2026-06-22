using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Message
{
    public class AttachmentsRequiredForPrepareException()
        : ApiExceptionBase(
            400,
            MessageErrors.AttachmentsRequiredForPrepare,
            "Prepare step requires at least one attachment")
    {
    }
}