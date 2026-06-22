using AIChatWebServer.Integrations.AI.Interfaces;
using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Integrations.AI.Implementations
{
    public class AIControllerFactory(IServiceProvider serviceProvider) : IAIControllerFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public IAIController Create(AIModel model, out string modelName)
        {
            var resolved = AIModelResolver.Resolve(model);

            modelName = resolved.ModelName;

            return (IAIController)_serviceProvider.GetRequiredService(resolved.ControllerType);
        }
    }
}