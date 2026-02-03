using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.User
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PreferenceGender
    {
        Male,
        Female,
        Any
    }
}
