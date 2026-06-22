using AIChatWebServer.Models.AI;
using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Payment
{
    public sealed record ModelAtributes([property: JsonPropertyName("model")] int model)
    {
        public AIModel Model => (AIModel)model;
    }
}
