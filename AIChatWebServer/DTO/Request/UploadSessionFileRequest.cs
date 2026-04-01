using AIChatWebServer.Models.Files;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record UploadSessionFileRequest
    {
        [JsonPropertyName("expectedFileName")]
        public required string ExpectedFileName { get; init; }
        [JsonPropertyName("expectedFileType")]
        public required FileType ExpectedFileType { get; init; }
        [JsonPropertyName("expectedFileSize")]
        public required long ExpectedFileSize { get; init; }
    }
}
