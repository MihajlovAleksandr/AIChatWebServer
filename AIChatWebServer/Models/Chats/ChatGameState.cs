using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Chats
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChatGameState
    {
        Win,
        Lose,
        Pending
    }
}
