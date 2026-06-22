using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Chats.RandomChat
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AiRole
    {
        RealAi = 0,
        FakeAi = 1
    }
}
