using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Utils.AI;

namespace AIChatWebServer.Services.Implementations.AI
{
    public class AIMessageCompressor(IAIMessageSender sender) : IAIMessageCompressor
    {
        private readonly IAIMessageSender _sender = sender;

        public async Task<AIMessage> Compress(
            IReadOnlyList<AIMessage> messages,
            CancellationToken cancellationToken = default)
        {
            if (messages == null || messages.Count == 0)
                throw new ArgumentException("Messages cannot be empty", nameof(messages));

            var chatId = messages.First().ChatId;

            string result;

            if (messages.Count == 1)
            {
                result = await _sender.SendAsync(
                    chatId,
                    AIModel.DeepSeekChat,
                    TokenOperation.SystemCompressMessage,
                    AIPrompts.SystemCompressMessage.ToString(),
                    new[]
                    {
                        new AIMessage(
                            chatId,
                            AIMessageRole.User,
                            AIMessageType.Message,
                            messages[0].Content)
                    },
                    cancellationToken);
            }
            else
            {
                result = await _sender.SendAsync(
                    chatId,
                    AIModel.DeepSeekChat,
                    TokenOperation.SystemCompressDialog,
                    AIPrompts.SystemCompressDialog.ToString(),
                    new[]
                    {
                        new AIMessage(
                            chatId,
                            AIMessageRole.User,
                            AIMessageType.Message,
                            BuildDialogText(messages))
                    },
                    cancellationToken);
            }

            return new AIMessage(
                chatId,
                AIMessageRole.System,
                AIMessageType.Compressed,
                result);
        }

        private string BuildDialogText(IEnumerable<AIMessage> messages)
        {
            return string.Join("\n", messages.Select(m =>
                $"{m.Role}: {m.Content}"));
        }
    }
}