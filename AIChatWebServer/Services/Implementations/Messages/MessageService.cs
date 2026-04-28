using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.File;
using AIChatWebServer.Models.Exceptions.Implementations.Message;
using AIChatWebServer.Models.Files;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Messages;
using FileNotFoundException = AIChatWebServer.Models.Exceptions.Implementations.File.FileNotFoundException;

namespace AIChatWebServer.Services.Implementations.Messages
{
    public class MessageService(
        IMessageRepository messageRepository,
        IChatService chatService, 
        IUploadSessionService uploadSessionService,
        IConversationActionValidator conversationActionValidator,
        IMessageVisibilityPolicy messageVisibilityPolicy,
        IFileService fileService) : IMessageService
    {
        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly IChatService _chatService = chatService;
        private readonly IUploadSessionService _uploadSessionService = uploadSessionService;
        private readonly IConversationActionValidator _conversationActionValidator = conversationActionValidator;
        private readonly IMessageVisibilityPolicy _messageVisibilityPolicy = messageVisibilityPolicy;
        private readonly IFileService _fileService = fileService;

        public async Task<UploadSession> PrepareAsync(
            Guid chatId, Guid userId, int textLength,
            IReadOnlyCollection<UploadSessionFile> files, 
            int repliesCount, CancellationToken ct)
        {
            if (files.Count == 0)
                throw new AttachmentsRequiredForPrepareException();

            Chat chat = await _chatService.GetById(chatId, ct);
            if (chat.IsChatEnded())
                throw new ChatAlreadyEndedException(chat.Id);
            _conversationActionValidator.Validate(chat,
                new PrepareSendMessageAction(userId, textLength, 
                    files.Select(f => f.ExpectedFileType).ToList(), 
                    repliesCount));

            return await _uploadSessionService.CreateSessionAsync(userId, UploadSessionPurpose.Message, files, ct);
        }

        public async Task<MessageContext> CreateAsync(
            Guid messageId,
            Chat chat,
            Guid userId,
            string text,
            Guid? uploadSessionId,
            IReadOnlyCollection<MessageReply> replies,
            CancellationToken ct)
        {
            if (chat.IsChatEnded())
                throw new ChatAlreadyEndedException(chat.Id);
            IReadOnlyCollection<UploadSessionFile> files;
            IReadOnlyCollection<Guid> fileIds;

            if (uploadSessionId != null)
            {
                files = await GetFilesBySessionId(uploadSessionId.Value, userId, UploadSessionPurpose.Message, ct);
                fileIds = files.Select(f => f.FileId ?? throw new UploadSessionNotCompletedException(uploadSessionId.Value)).ToList();
            }
            else
            {
                files = [];
                fileIds = [];
            }

            if (text.Length == 0 && files.Count == 0)
                throw new EmptyMessageException();

            _conversationActionValidator.Validate(chat,
                new SendMessageAction(userId, text, files, replies));

            await ValidateMessageReplies(replies, ct);

            var userInChat = chat.UsersWithData.Keys;

            await _messageRepository.CreateAsync(
                messageId,
                chat.Id,
                userId,
                text,
                userInChat,
                fileIds,
                replies,
                ct);

            var message = await _messageRepository.GetByIdWithDependenciesAsync(messageId, ct)
                ?? throw new MessageNotFoundException(messageId);

            return ToResponse(message, userId, chat.Type);
        }

        public async Task<MessageContext> GetById(Guid messageId, Guid userId, CancellationToken ct)
        {
            Message message =  await _messageRepository.GetByIdWithDependenciesAsync(messageId, ct)
                ?? throw new MessageNotFoundException(messageId);

            Chat chat = await _chatService.GetById(message.ChatId, ct);
            if (!chat.UsersWithData.ContainsKey(userId))
                throw new UserDoesNotBelongToChatException(chat.Id, userId);

            return ToResponse(message, userId, chat.Type);
        }

        public async Task<IEnumerable<MessageContext>> GetByChatId(Guid chatId, Guid userId, CancellationToken ct)
        {
            Chat chat = await _chatService.GetById(chatId, ct);

            if (!chat.UsersWithData.ContainsKey(userId))
                throw new UserDoesNotBelongToChatException(chat.Id, userId);

            var messages = await _messageRepository.GetByChatWithDependenciesAsync(chatId, ct);

            return messages.Select(m => ToResponse(m, userId, chat.Type));

        }

        public async Task EditText(Guid messageId, string text, Guid userId, CancellationToken ct)
        {
            Message message = await _messageRepository.GetByIdWithDependenciesAsync(messageId, ct)
                ?? throw new MessageNotFoundException(messageId);
            if (message.UserId != userId)
            {
                throw new CannotEditForeignMessageException(messageId, userId);
            }
            if (string.IsNullOrEmpty(text) && message.Files.Count == 0)
            {
                throw new EmptyMessageException();
            }

            Chat chat = await _chatService.GetById(message.ChatId, ct);
            if (chat.IsChatEnded())
                throw new ChatAlreadyEndedException(chat.Id);
            _conversationActionValidator.Validate(chat, new EditMessageAction(message, userId, text));

            await _messageRepository.UpdateTextAsync(messageId, text, DateTime.UtcNow, ct);
        }

        public async Task DeleteFile(Guid messageId, Guid fileId, Guid userId, CancellationToken ct)
        {
            Message message = await _messageRepository.GetByIdWithDependenciesAsync(messageId, ct)
                ?? throw new MessageNotFoundException(messageId);
            var files = message.Files.Where(f => f.Id == fileId);
            FileModel fileModel = files.FirstOrDefault()
                ?? throw new FileNotFoundException(fileId);
            Chat chat = await _chatService.GetById(message.ChatId, ct);
            if (chat.IsChatEnded())
                throw new ChatAlreadyEndedException(chat.Id);
            _conversationActionValidator.Validate(chat, new DeleteMessageAction(message, userId));
            await _messageRepository.DeleteFileAsync(messageId, fileId, ct);
            fileModel.RemoveUsage();
            if (fileModel.ReferenceCount == 0)
            {
                await _fileService.DeleteAsync(fileId, ct);
            }

        }

        public async Task DeleteMessage(Guid messageId, Guid userId, CancellationToken ct)
        {
            Message message = await _messageRepository.GetByIdWithDependenciesAsync(messageId, ct)
                ?? throw new MessageNotFoundException(messageId);

            Chat chat = await _chatService.GetById(message.ChatId, ct);
            if (chat.IsChatEnded())
                throw new ChatAlreadyEndedException(chat.Id);
            _conversationActionValidator.Validate(chat, new DeleteMessageAction(message, userId));

            await _messageRepository.DeleteMessageAsync(messageId, ct);
        }

        public async Task EditMessagesStatus(IReadOnlyCollection<Guid> messageIds, Guid chatId, MessageStatus status, Guid userId, CancellationToken ct)
        {
            foreach (var messageId in messageIds) {
                Message message = await _messageRepository.GetById(messageId, ct)
                    ?? throw new MessageNotFoundException(messageId);

                if (chatId != message.ChatId)
                    throw new MessageDoesNotBelongToChatException(messageId, chatId);

                if (message.UserId == userId) {
                    throw new CannotUpdateOwnMessageStatusException(messageId, userId);
                }

                await _messageRepository.UpdateStatusAsync(messageId, userId, status, DateTime.UtcNow, ct);
            }
        }

        private async Task<IReadOnlyCollection<UploadSessionFile>> GetFilesBySessionId(
            Guid sessionId, Guid userId,
            UploadSessionPurpose purpose,
            CancellationToken ct)
        {
            UploadSession session = await _uploadSessionService.GetByIdAsync(sessionId, ct)
                ?? throw new UploadSessionNotFoundException(sessionId);

            await ValidateUploadSession(session, userId, purpose, ct);

            return session.Files;
        }

        private async Task ValidateUploadSession(
            UploadSession session, Guid userId, 
            UploadSessionPurpose purpose,
            CancellationToken ct)
        {
             session = await
                   _uploadSessionService.GetByIdAsync(session.Id, ct)
                       ?? throw new UploadSessionNotFoundException(session.Id);
            if (session.Status != UploadSessionStatus.Completed)
                throw new UploadSessionNotCompletedException(session.Id);

            if (session.UserId != userId)
                throw new UploadSessionAccessDeniedException(session.Id, userId);
            if (session.Purpose != purpose)
                throw new UploadSessionInvalidPurposeException(session.Id, purpose);
        }

        private async Task ValidateMessageReplies(IReadOnlyCollection<MessageReply> messageReplies, CancellationToken ct)
        {
            foreach (var messageReply in messageReplies)
            {
                Message replyToMessage =
                    await _messageRepository.GetById(messageReply.ReplyMessageId, ct)
                        ?? throw new MessageReplyNotFoundException(messageReply.ReplyMessageId);

                if (messageReply.StartIndexQuote != null
                    && messageReply.EndIndexQuote != null)
                {
                    if (messageReply.StartIndexQuote > messageReply.EndIndexQuote)
                    {
                        throw new InvalidQuoteRangeException(
                            messageReply.StartIndexQuote,
                            messageReply.EndIndexQuote);
                    }

                    if (messageReply.StartIndexQuote > replyToMessage.Text.Length
                        || messageReply.EndIndexQuote > replyToMessage.Text.Length)
                    {
                        throw new QuoteIndexOutOfRangeException(
                            messageReply.StartIndexQuote, 
                            messageReply.EndIndexQuote,
                            replyToMessage.Text.Length);
                    }
                }
            }
        }

        public async Task<SyncMessages> SyncAsync(
            Guid userId,
            DateTime since,
            CancellationToken ct)
        {
            IReadOnlyCollection<Chat> chats = await _chatService.GetByUserId(userId, ct);

            foreach (var chat in chats)
            {
                if (!chat.UsersWithData.ContainsKey(userId))
                    throw new UserDoesNotBelongToChatException(chat.Id, userId);
            }

            var chatIds = chats.Select(c => c.Id).ToList();

            var changes = await _messageRepository.GetChangesAsync(chatIds, since, ct);

            var chatsMap = chats.ToDictionary(c => c.Id);

            var newMessages = changes.Created
                .Select(m => ToResponse(m, userId, chatsMap[m.ChatId].Type))
                .ToList();

            var updatedMessages = changes.Updated
                .Select(m => ToResponse(m, userId, chatsMap[m.ChatId].Type))
                .ToList();

            return new SyncMessages(
                newMessages,
                updatedMessages,
                changes.Deleted
            );
        }

        private MessageContext ToResponse(Message message, Guid userId, ChatType chatType)
        {
            return new MessageContext(message, userId, _messageVisibilityPolicy.ShouldIncludeStatuses(chatType));
        }
    }
}
