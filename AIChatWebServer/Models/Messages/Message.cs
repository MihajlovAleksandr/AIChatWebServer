using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Models.Messages
{

    public sealed class Message(
        Guid id,
        Guid chatId,
        Guid userId,
        string text,
        DateTime time,
        DateTime lastUpdate,
        IReadOnlyDictionary<Guid, MessageStatus> statuses,
        IReadOnlyCollection<MessageReply> replies,
        IReadOnlyCollection<FileModel> files)
    {
        public Guid Id { get; init; } = id;

        public Guid ChatId { get; init; } = chatId;

        public Guid UserId { get; init; } = userId;

        public string Text { get; init; } = text;

        public DateTime Time { get; init; } = time;

        public DateTime LastUpdate { get; init; } = lastUpdate;

        public IReadOnlyDictionary<Guid, MessageStatus> Statuses { get; init; } = statuses;

        public IReadOnlyCollection<MessageReply> Replies { get; init; } = replies;

        public IReadOnlyCollection<FileModel> Files { get; init; } = files;
    }
}