namespace AIChatWebServer.Integrations.AI.Interfaces
{
    public interface IAIControllerFactory
    {
        IAIController Create(AIModel model, out string modelName);
    }
}
