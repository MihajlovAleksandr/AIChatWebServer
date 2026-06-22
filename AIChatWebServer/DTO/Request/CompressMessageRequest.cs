using AIChatWebServer.Models.AI;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public class CompressMessageRequest
    {
        public Guid ChatId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AIModel Model { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}