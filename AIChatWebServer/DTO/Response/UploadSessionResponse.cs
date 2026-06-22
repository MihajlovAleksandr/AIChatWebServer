using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record UploadSessionResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("files")] IReadOnlyCollection<UploadSessionFileResponse> Files
    );
}
