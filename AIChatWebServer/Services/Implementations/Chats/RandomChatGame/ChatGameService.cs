namespace AIChatWebServer.Services.Implementations.Chats.RandomChatGame
{
    using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
    using AIChatWebServer.Models.Chats;
    using AIChatWebServer.Models.Chats.RandomChat;
    using AIChatWebServer.Models.Exceptions.Implementations.Chat.RandomChatGame;
    using AIChatWebServer.Repositories.Interfaces;
    using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;

    public sealed class ChatGameService(
        IChatGameRepository repository,
        IChatGameResultProcessor gameResultProcessor,
        IUnitOfWorkFactory unitOfWorkFactory)
        : IChatGameService
    {
        private readonly IChatGameRepository _repository = repository;
        private readonly IChatGameResultProcessor _gameResultProcessor = gameResultProcessor;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;

        public async Task<ChatGameSession> CreateGameAsync(
            Guid chatId,
            Guid guesserUserChatId,
            Guid opponentUserChatId,
            AiRole opponentAiRole,
            CancellationToken ct = default)
        {
            if (guesserUserChatId == opponentUserChatId)
                throw new ChatGameInvalidPlayersException();

            var uow = await _unitOfWorkFactory.CreateAsync(ct);
            var repository = uow.WithTransaction(_repository);

            var existing = await repository.GetByChatIdAsync(chatId, ct);
            if (existing is not null)
                throw new ChatGameAlreadyExistsException(chatId);

            var sessionId = await repository.CreateAsync(
                chatId,
                guesserUserChatId,
                opponentUserChatId,
                opponentAiRole,
                ct);

            var session = await repository.GetByIdAsync(sessionId, ct);
            await uow.CommitAsync(ct);

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
            Guid chatId,
            AiRole guessedAiRole,
            CancellationToken ct = default)
        {
            var uow = await _unitOfWorkFactory.CreateAsync(ct);
            var repository = uow.WithTransaction(_repository);

            var session = await repository.GetByChatIdAsync(chatId, ct)
                ?? throw new ChatGameNotFoundException(chatId);

            if (session.IsFinished)
                throw new ChatGameAlreadyFinishedException(session.Id);

            await repository.SetResultAsync(session.Id, guessedAiRole, ct);

            var updated = await repository.GetByIdAsync(session.Id, ct) 
                ?? throw new ChatGameNotFoundException(session.Id); ;


            var gameResultProcessor = uow.WithTransaction(_gameResultProcessor);

            await gameResultProcessor.ProcessAsync(updated, ct);

            await uow.CommitAsync(ct);
            return updated;
        }

        public async Task<ChatGameState> GetResultAsync(
            Guid sessionId,
            CancellationToken ct = default)
        {
            var session = await GetByIdAsync(sessionId, ct);

            if (!session.IsFinished)
                return ChatGameState.Pending;

            return session.IsCorrect()!.Value ? ChatGameState.Win : ChatGameState.Lose;
        }
    }
}