using AIChatWebServer.Models.Files;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record UploadFileRequest
    {
        [JsonPropertyName("fileType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required FileType FileType { get; init; }
    }
}
