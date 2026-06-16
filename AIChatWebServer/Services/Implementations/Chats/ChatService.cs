using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;

namespace AIChatWebServer.Services.Implementations.Chats
{
    public class ChatService(IChatRepository chatRepository, IConversationActionValidator chatActionValidator) : IChatService
    {
        private readonly IChatRepository _chatRepository = chatRepository;
        private readonly IConversationActionValidator _chatActionValidator = chatActionValidator;

        public async Task<Guid> CreateAsync(ChatType type, IDictionary<Guid, string> creatorsWithChatNames, CancellationToken cancellationToken = default)
        {
            return await _chatRepository.CreateAsync(type, creatorsWithChatNames, cancellationToken);
        }

        public async Task<Chat> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            return await _chatRepository.GetById(id, cancellationToken) ?? throw new ChatNotFoundException(id);
        }

        public async Task ExecuteAction(
            Guid chatId,
            ChatAction action,
            CancellationToken cancellationToken = default)
        {
            Chat chat = await _chatRepository.GetById(chatId, cancellationToken)
                ?? throw new ChatNotFoundException(chatId);

            _chatActionValidator.Validate(chat, action);

            await ApplyAction(chatId, action, cancellationToken);
        }

        private Task ApplyAction(
            Guid chatId,
            ChatAction action,
            CancellationToken cancellationToken)
        {
            return action switch
            {
                UpdateNameAction updateNameAction =>
                    _chatRepository.UpdateName(chatId, updateNameAction.UserId, updateNameAction.Name, cancellationToken),

                AddUserAction addUserAction =>
                    _chatRepository.AddUser(chatId, addUserAction.AddedUserId, addUserAction.ChatName, addUserAction.RoleOnJoin, cancellationToken),

                RemoveUserAction removeUserAction =>
                    _chatRepository.RemoveUser(chatId, removeUserAction.RemovedUserId, cancellationToken),

                EndChatAction =>
                    _chatRepository.End(chatId, DateTime.UtcNow, cancellationToken),

                ChangeChatSettingsAction changeChatSettingsAction =>
                    _chatRepository.UpdateChatSettings(chatId, changeChatSettingsAction.NewSettings,
                        cancellationToken),

                ChangeUserSettingsAction changeUserSettingsAction =>
                    _chatRepository.UpdateUserSettings(chatId, changeUserSettingsAction.TargetUserId,
                        changeUserSettingsAction.NewSettings, cancellationToken),

                _ => throw new NotSupportedException()
            };
        }

        public async Task<IReadOnlyList<Chat>> GetByUserId(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _chatRepository.GetByUserId(userId, cancellationToken);
        }

        public async Task<SyncChats> SyncAsync(
            Guid userId,
            DateTime since,
            CancellationToken ct)
        {
            var changes = await _chatRepository.GetChangesAsync(userId, since, ct);

            return new SyncChats(
                changes.Created.Select(c=>new ChatWithUserContext(c, userId)).ToList(),
                changes.Updated.Select(c => new ChatWithUserContext(c, userId)).ToList(),
                changes.Deleted
            );
        }

        public async Task<Guid> GetUserIdByChatUserId(Guid chatUserId, CancellationToken ct = default)
        {
            return await _chatRepository.GetUserIdByChatUserId(chatUserId, ct)
                ?? throw new ChatUserNotFoundException(chatUserId);
        }
    }
}
