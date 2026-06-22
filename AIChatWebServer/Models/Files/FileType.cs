using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Files
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FileType
    {
        MessageImage = 1,
        MessageFile = 2,
        VoiceMessage = 3,
        VideoMessage = 4
    }
}
