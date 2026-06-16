using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.AI
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AIModel
    {
        Default = DeepSeekChat,
        DeepSeekChat = 1,
        OllamaQwen3_4B = 2,
        OllamaLlama3 = 3,
        OllamaMistral = 4,
        OllamaGemma4е = 5,
    }
}
