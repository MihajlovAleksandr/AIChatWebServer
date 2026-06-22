using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public class PrepareCreateThemeRequest
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("file")]
        public required UploadSessionFileRequest File { get; init; }
    }
}
