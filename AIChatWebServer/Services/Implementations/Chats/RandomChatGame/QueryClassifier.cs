using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;
using AIChatWebServer.Utils.AI;

namespace AIChatWebServer.Services.Implementations.Chats.RandomChatGame
{
    public sealed class QueryClassifier(
        IAIMessageSender aiClient,
        IQueryTagParser parser) : IQueryClassifier
    {
        private readonly IQueryTagParser _parser = parser;
        private readonly IAIMessageSender _ai = aiClient;

        public async Task<QueryTags> ClassifyAsync(
            Guid chatId,
            string message,
            CancellationToken ct = default)
        {
            var prompt = AIPrompts.ClassifyQuery
                .IncrementInputs(message);

            var response = await _ai.SendAsync(chatId, AIModel.Default, TokenOperation.ClassifyQuery, prompt, ct: ct);

            return _parser.Parse(response);
        }
    }
}