using AIChatWebServer.Models.Files;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record UploadSessionFileResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("expectedFileName")] string ExpectedFileName,
        [property: JsonPropertyName("expectedFileType")] FileType ExpectedFileType,
        [property: JsonPropertyName("expectedFileSize")] long ExpectedFileSize
    );
}
