using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Payment
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentStatuses
    {
       Pending,
       Confirmed,
       Failed,
       Expired
    }
}
