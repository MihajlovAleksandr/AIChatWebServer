using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Integrations.AI.Implementations
{
    public static class AIModelResolver
    {
        public static (Type ControllerType, string ModelName) Resolve(AIModel model)
        {
            return model switch
            {
                AIModel.DeepSeekChat => (typeof(DeepSeekController), "deepseek-chat"),

                AIModel.OllamaQwen3_4B => (typeof(OllamaController), "qwen3:4b"),
                AIModel.OllamaLlama3 => (typeof(OllamaController), "llama3"),
                AIModel.OllamaMistral => (typeof(OllamaController), "mistral"),
                AIModel.OllamaGemma4е => (typeof(OllamaController), "gemma4:e2b"),

                _ => throw new NotSupportedException($"Model {model} not supported")
            };
        }
    }
}
