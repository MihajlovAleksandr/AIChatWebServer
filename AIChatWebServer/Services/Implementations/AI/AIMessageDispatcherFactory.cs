using AIChatWebServer.Models.AI;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;
using Microsoft.Extensions.Options;

namespace AIChatWebServer.Services.Implementations.AI
{
    public class AIMessageDispatcherFactory(
        IOptions<AISettings> settings,
        IAIMessageRepository repository,
        IAIMessageCompressor compressor) : IAIMessageDispatcherFactory
    {
        private readonly IOptions<AISettings> _settings = settings;
        private readonly IAIMessageRepository _repository = repository;
        private readonly IAIMessageCompressor _compressor = compressor;

        public IAIMessageDispatcher Create(Guid chatId)
        {
            return new AIMessageDispatcher(
                chatId,
                _settings,
                _repository,
                _compressor);
        }
    }
}