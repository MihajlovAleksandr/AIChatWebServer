namespace AIChatWebServer.Models.Chats.Matchmaking
{
    public sealed record GroupMatchmakingResult : ChatMatchmakingResult
    {
        public Guid UserId { get; init; }
        public GroupMatchmakingResult(Guid ChatId, Guid UserId) : base(ChatId)
        {
            this.UserId = UserId;
        }
    }
}
