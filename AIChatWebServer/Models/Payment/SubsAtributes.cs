using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Payment
{
    public record SubsAtributes
    (
        [property: JsonPropertyName("subsPeriodDays")] int Days
    );
}
