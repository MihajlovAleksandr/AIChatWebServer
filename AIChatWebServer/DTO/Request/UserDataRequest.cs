using AIChatWebServer.Models.User;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record UserDataRequest
    (
        [property: JsonPropertyName("gender")]
        [property: JsonConverter(typeof(JsonStringEnumConverter))]
        Gender Gender,
        [property: JsonPropertyName("age")] int Age,
        [property: JsonPropertyName("name")] string Name
    );
}