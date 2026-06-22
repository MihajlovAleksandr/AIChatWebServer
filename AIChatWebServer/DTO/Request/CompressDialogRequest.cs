using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Messages;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public class CompressDialogRequest
    {
        public Guid ChatId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AIModel Model { get; set; }
        public IEnumerable<Message> Messages { get; set; } = new List<Message>();
    }
}