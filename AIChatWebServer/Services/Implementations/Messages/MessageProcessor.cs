using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Implementations.Messages.Processors;
using AIChatWebServer.Services.Interfaces.Messages;

namespace AIChatWebServer.Services.Implementations.Messages
{
    public class MessageProcessorFactory(IServiceProvider provider) : IMessageProcessorFactory
    {
        private readonly IServiceProvider _provider = provider;

        public IMessageProcessor? Create(ChatType chatType)
        {
            return chatType switch
            {
                ChatType.AI => _provider.GetRequiredService<AIChatMessageProcessor>(),
                ChatType.Random => _provider.GetRequiredService<RandomMessageProcessor>(),
                _ => null
            };
        }
    }
}