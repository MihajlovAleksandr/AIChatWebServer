using AIChatWebServer.Models.Files;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record UploadFileRequest
    {
        [JsonPropertyName("sessionId")]
        public Guid SessionId { get; init; }
        [JsonPropertyName("fileId")]
        public Guid FileId {  get; init; }
        [JsonPropertyName("fileType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required FileType FileType { get; init; }
    }
}
