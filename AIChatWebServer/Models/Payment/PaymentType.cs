using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Payment
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentType
    {
        Subscription,
        Payment,
        SingleItem
    }
}
