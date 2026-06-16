namespace AIChatWebServer.Models.Chats.RandomChat;

public sealed record ChatGameParticipant
{
    public Guid Id { get; init; }
    public Guid UserChatId { get; init; }
    public GameRole GameRole { get; init; }
    public AiRole? AiRole { get; init; }

    public ChatGameParticipant(
        Guid id,
        Guid userChatId,
        GameRole gameRole,
        AiRole? aiRole)
    {
        Id = id;
        UserChatId = userChatId;
        GameRole = gameRole;
        AiRole = aiRole;

        Validate();
    }

    private void Validate()
    {
        if (GameRole == GameRole.Guesser && AiRole is not null)
            throw new InvalidOperationException(
                "Guesser must not have AiRole");

        if (GameRole == GameRole.Opponent && AiRole is null)
            throw new InvalidOperationException(
                "Opponent must have AiRole");
    }
}