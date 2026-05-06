using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record CreatePaymentResponse
        (
            [property: JsonPropertyName("id")] Guid Id,
            [property: JsonPropertyName("amount")] decimal Amount,
            [property: JsonPropertyName("currency")] string Currency,
            [property: JsonPropertyName("items")] IEnumerable<PaymentItemResponse> Items
        );
}
