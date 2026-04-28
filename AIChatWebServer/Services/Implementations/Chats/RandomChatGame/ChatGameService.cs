namespace AIChatWebServer.Services.Implementations.Chats.RandomChatGame
{
    using AIChatWebServer.Models.Chats.RandomChat;
    using AIChatWebServer.Models.Exceptions.Implementations.Chat.RandomChatGame;
    using AIChatWebServer.Repositories.Interfaces;
    using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;

    public sealed class ChatGameService(
        IChatGameRepository repository)
        : IChatGameService
    {
        private readonly IChatGameRepository _repository = repository;

        public async Task<ChatGameSession> CreateGameAsync(
            Guid chatId,
            Guid guesserUserChatId,
            Guid opponentUserChatId,
            AiRole opponentAiRole,
            CancellationToken ct = default)
        {
            if (guesserUserChatId == opponentUserChatId)
                throw new ChatGameInvalidPlayersException();

            var existing = await _repository.GetByChatIdAsync(chatId, ct);
            if (existing is not null)
                throw new ChatGameAlreadyExistsException(chatId);

            var sessionId = await _repository.CreateAsync(
                chatId,
                guesserUserChatId,
                opponentUserChatId,
                opponentAiRole,
                ct);

            var session = await _repository.GetByIdAsync(sessionId, ct);

            return session ?? throw new ChatGameNotFoundException(sessionId);
        }

        public async Task<ChatGameSession> GetByChatIdAsync(
            Guid chatId,
            CancellationToken ct = default)
        {
            var session = await _repository.GetByChatIdAsync(chatId, ct);

            return session ?? throw new ChatGameNotFoundException(chatId);
        }

        public async Task<ChatGameSession> GetByIdAsync(
            Guid sessionId,
            CancellationToken ct = default)
        {
            var session = await _repository.GetByIdAsync(sessionId, ct);

            return session ?? throw new ChatGameNotFoundException(sessionId);
        }

        public async Task<ChatGameSession> MakeGuessAsync(
            Guid sessionId,
            AiRole guessedAiRole,
            CancellationToken ct = default)
        {
            var session = await GetByIdAsync(sessionId, ct);

            if (session.IsFinished)
                throw new ChatGameAlreadyFinishedException(sessionId);

            await _repository.SetResultAsync(sessionId, guessedAiRole, ct);

            var updated = await _repository.GetByIdAsync(sessionId, ct);

            return updated ?? throw new ChatGameNotFoundException(sessionId);
        }

        public async Task<bool> GetResultAsync(
            Guid sessionId,
            CancellationToken ct = default)
        {
            var session = await GetByIdAsync(sessionId, ct);

            if (!session.IsFinished)
                throw new ChatGameNotFinishedException(sessionId);

            return session.IsCorrect()!.Value;
        }
    }
}