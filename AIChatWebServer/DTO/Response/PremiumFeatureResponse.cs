using AIChatWebServer.Models.User;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record PremiumFeatureResponse
    (
        [property: JsonPropertyName("feature"),
            JsonConverter(typeof(JsonStringEnumConverter))] PremiumFeature PremiumFeature
    );
}
