using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.User
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RegistrationState
    {
        Created = 0,
        EmailVerified = 1,
        UserDataCompleted = 2,
        PreferenceCompleted = 3,
        Completed = 4
    }
}
