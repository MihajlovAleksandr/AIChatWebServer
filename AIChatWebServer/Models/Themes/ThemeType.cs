using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Themes
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ThemeType
    {
        System = 1,
        Custom = 2
    }
}
