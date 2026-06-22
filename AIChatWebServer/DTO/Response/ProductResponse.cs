using AIChatWebServer.Models.Payment;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record ProductResponse
        (
            [property: JsonPropertyName("id")] Guid Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("paymentType")] PaymentType PaymentType,
            [property: JsonPropertyName("price")] decimal Price,
            [property: JsonPropertyName("currency")] string Currency,
            [property: JsonPropertyName("description")] string Description
        );
}
