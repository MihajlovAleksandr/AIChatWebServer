namespace AIChatWebServer.Integrations.AI.DTO
{
    public class AIMessageResponse(string answer, int totalTokenUsed)
    {
        public string Answer { get; set; } = answer;
        public int TotalTokensUsed { get; set; } = totalTokenUsed;
    }
}
