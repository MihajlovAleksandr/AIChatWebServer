using AIChatWebServer.Models.AI;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;
using Microsoft.Extensions.Options;

namespace AIChatWebServer.Services.Implementations.AI
{
    public class AIMessageDispatcher(
        Guid chatId,
        IOptions<AISettings> settings,
        IAIMessageRepository repository,
        IAIMessageCompressor compressor) : IAIMessageDispatcher
    {
        private readonly AISettings _settings = settings.Value;
        private readonly IAIMessageRepository _repository = repository;
        private readonly IAIMessageCompressor _compressor = compressor;
        private readonly Guid _chatId = chatId;

        private readonly List<AIMessage> _messages = new();
        private readonly List<AIMessage> _compressedMessages = new();

        public void LoadFromHistory(IEnumerable<AIMessage> messages)
        {
            foreach (var message in messages)
            {
                if (message.Type == AIMessageType.Compressed)
                {
                    _compressedMessages.Add(message);
                }
                else
                {
                    _messages.Add(message);
                }
            }
        }

        public async Task AddMessage(AIMessage message, CancellationToken ct = default)
        {
            _messages.Add(message);

            await HandleMessagesOverflow(ct);
        }

        public IReadOnlyList<AIMessage> GetMessages() => _messages;

        public IReadOnlyList<AIMessage> GetCompressedMessages() => _compressedMessages;

        private async Task HandleMessagesOverflow(CancellationToken ct)
        {
            var settings = _settings.AIMessageBuffers.Messages;

            if (_messages.Count <= settings.Max)
                return;

            var toCompress = _messages
                .Take(_messages.Count - settings.Min)
                .ToList();

            _messages.RemoveRange(0, toCompress.Count);

            var compressed = await _compressor.Compress(toCompress, ct);
            
            foreach(var message in toCompress)
            {
                await _repository.Delete(message.Id, ct);
            }

            _compressedMessages.Add(compressed);

            await _repository.Add(
                _chatId,
                compressed.Role,
                compressed.Type,
                compressed.Content,
                ct);

            await HandleCompressedOverflow(ct);
        }

        private async Task HandleCompressedOverflow(CancellationToken ct)
        {
            var settings = _settings.AIMessageBuffers.CompressedMessages;

            if (_compressedMessages.Count <= settings.Max)
                return;

            var toCompress = _compressedMessages
                .Take(_compressedMessages.Count - settings.Min)
                .ToList();

            _compressedMessages.RemoveRange(0, toCompress.Count);

            foreach (var message in toCompress)
            {
                await _repository.Delete(message.Id, ct);
            }

            var compressed = await _compressor.Compress(toCompress, ct);

            _compressedMessages.Add(compressed);

            await _repository.Add(
                _chatId,
                compressed.Role,
                compressed.Type,
                compressed.Content,
                ct);
        }
    }
}