using AIChatWebServer.Models.User;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record UserDataResponse
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("age")] int Age,
        [property: JsonPropertyName("gender")] Gender Gender
    );
}
