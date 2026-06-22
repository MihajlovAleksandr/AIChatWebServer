namespace AIChatWebServer.Models.AI
{
    public record AISettingsWithModels
    (
        AISettingsModel Settings,
        IReadOnlyCollection<AIModel> Models
    );
}
