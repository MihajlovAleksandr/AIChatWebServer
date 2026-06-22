using AIChatWebServer.Models.AI;
using System.Text.Json.Serialization;

namespace AIChatWebServer.Integrations.AI.DTO
{
    public class AIMessageRequest
    {
        public AIMessageRequest(string content, string role)
        {
            Role = role;
            Content = content;
        }

        public AIMessageRequest(AIMessage aIMessage)
        {
            Role = aIMessage.Role.ToString();
            Content = aIMessage.Content;
        }

        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
