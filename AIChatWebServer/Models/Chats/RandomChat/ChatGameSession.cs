namespace AIChatWebServer.Models.Chats.RandomChat
{
    public sealed record ChatGameSession
    {
        public Guid Id { get; init; }
        public Guid ChatId { get; init; }
        public AiRole? GuessedAiRole { get; init; }

        public IReadOnlyCollection<ChatGameParticipant> Participants { get; init; }

        public ChatGameSession(
            Guid id,
            Guid chatId,
            AiRole? guessedAiRole,
            IReadOnlyCollection<ChatGameParticipant> participants)
        {
            Id = id;
            ChatId = chatId;
            GuessedAiRole = guessedAiRole;
            Participants = participants ?? throw new ArgumentNullException(nameof(participants));

            Validate();
        }

        private void Validate()
        {
            if (Participants.Count != 2)
                throw new InvalidOperationException(
                    "Game must contain exactly 2 participants");

            if (Participants.Count(p => p.GameRole == GameRole.Guesser) != 1)
                throw new InvalidOperationException(
                    "Game must contain exactly one guesser");

            if (Participants.Count(p => p.GameRole == GameRole.Opponent) != 1)
                throw new InvalidOperationException(
                    "Game must contain exactly one opponent");
        }

        public ChatGameParticipant GetGuesser() =>
            Participants.Single(p => p.GameRole == GameRole.Guesser);

        public ChatGameParticipant GetOpponent() =>
            Participants.Single(p => p.GameRole == GameRole.Opponent);

        public bool IsFinished => GuessedAiRole is not null;

        public bool? IsCorrect()
        {
            if (GuessedAiRole is null)
                return null;

            var opponent = GetOpponent();

            return opponent.AiRole == GuessedAiRole;
        }

        public ChatGameParticipant? GetWinner()
        {
            var isCorrect = IsCorrect();
            if (isCorrect is null)
                return null;

            return isCorrect.Value
                ? GetGuesser()
                : GetOpponent();
        }
    }
}