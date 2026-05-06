using AIChatWebServer.Models.Payment;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record PaymentItemResponse
        (
            [property: JsonPropertyName("product")] ProductResponse Product,
            [property: JsonPropertyName("quantity")] int Quantity
        );
}
