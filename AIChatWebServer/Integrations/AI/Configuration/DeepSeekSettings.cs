namespace AIChatWebServer.Integrations.AI.Configuration
{
    public class DeepSeekSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = string.Empty;
        public int MaxTokenCount { get; set; }
    }
}