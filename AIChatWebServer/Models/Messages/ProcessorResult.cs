namespace AIChatWebServer.Models.Messages
{
    public record ProcessorResult(
        string Text,
        Guid SenderId
    );
}
