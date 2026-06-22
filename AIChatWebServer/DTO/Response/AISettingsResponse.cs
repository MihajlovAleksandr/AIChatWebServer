using AIChatWebServer.Models.AI;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record AISettingsResponse
    (
        [property: JsonPropertyName("aiModel")] AIModel AIModel,
        [property: JsonPropertyName("prompt")] string? Prompt,
        [property: JsonPropertyName("availableModels")] IReadOnlyCollection<AIModel> AvaibleModels
    );
}
