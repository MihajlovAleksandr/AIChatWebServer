using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.User
{
    public class UserPremium()
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }
        [JsonPropertyName("endTime")]
        public DateTime EndTime { get; set; }
    }
}
