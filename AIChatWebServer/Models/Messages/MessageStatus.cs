using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Messages
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageStatus
    {
        None = 0,
        Sent = 1,
        Read = 2
    }
}