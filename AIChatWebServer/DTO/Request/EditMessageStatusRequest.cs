using AIChatWebServer.Models.Messages;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record EditMessageStatusRequest
    {
        [JsonPropertyName("ids")] 
        public required IReadOnlyCollection<Guid> Ids { get; init; }
        [JsonPropertyName("status")] 
        public required MessageStatus Status { get; init; }
    }
}
