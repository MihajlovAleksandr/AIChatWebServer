using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Files;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Messages;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageController(
        IMessageService messageService,
        IMessageOrchestrator messageOrchestrator,
        IConnectionValidator connectionValidator,
        ICollectionRequestMapper<UploadSessionFileRequest, UploadSessionFile> sessionFileMapper,
        IResponseMapper<UploadSession, UploadSessionResponse> sessionMapper,
        ICollectionRequestMapper<MessageReplyRequest, MessageReply> messageRepliesMapper,
        IResponseMapper<MessageContext, MessageResponse> messageMapper,
        ICollectionResponseMapper<MessageContext, MessageResponse> messageCollectionMapper
    ) : ControllerBase
    {
        private readonly IMessageService _messageService = messageService;
        private readonly IMessageOrchestrator _messageOrchestrator = messageOrchestrator;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly ICollectionRequestMapper<UploadSessionFileRequest, UploadSessionFile> _sessionFileMapper = sessionFileMapper;
        private readonly IResponseMapper<UploadSession, UploadSessionResponse> _sessionMapper = sessionMapper;
        private readonly ICollectionRequestMapper<MessageReplyRequest, MessageReply> _messageRepliesMapper = messageRepliesMapper;
        private readonly IResponseMapper<MessageContext, MessageResponse> _messageMapper = messageMapper;
        private readonly ICollectionResponseMapper<MessageContext, MessageResponse> _messageCollectionMapper = messageCollectionMapper;

        [Authorize]
        [HttpPost("prepare")]
        public async Task<IActionResult> Prepare(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            [FromBody] PrepareSendMessageRequest request,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            UploadSession session = await _messageService.PrepareAsync(
                request.ChatId,
                tokenContext.UserId,
                request.TextLength,
                _sessionFileMapper.ToModel(request.Files),
                request.RepliesCount,
                ct);

            return Ok(_sessionMapper.ToResponse(session));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            [FromBody] MessageRequest request,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            MessageContext messageContext =
                await _messageOrchestrator.SendMessageAsync(
                    request.Id,
                    request.ChatId,
                    tokenContext.UserId,
                    tokenContext.ConnectionId,
                    request.Text,
                    request.UploadSessionId,
                    _messageRepliesMapper.ToModel(request.Replies),
                    ct);

            return Ok(_messageMapper.ToResponse(messageContext));
        }

        [Authorize]
        [HttpGet("{messageId}")]
        public async Task<IActionResult> GetById(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            [FromRoute] Guid messageId,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            var message = await _messageService.GetById(
                messageId,
                tokenContext.UserId,
                ct);

            return Ok(_messageMapper.ToResponse(message));
        }

        [Authorize]
        [HttpGet("chat/{chatId}")]
        public async Task<IActionResult> GetByChatId(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            [FromRoute] Guid chatId,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            var messages = await _messageService.GetByChatId(
                chatId,
                tokenContext.UserId,
                ct);

            return Ok(_messageCollectionMapper.ToResponse(messages));
        }

        [Authorize]
        [HttpPatch("{messageId}")]
        public async Task<IActionResult> EditText(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            [FromRoute] Guid messageId,
            [FromBody] EditTextRequest request,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            var message = await _messageOrchestrator.EditTextAsync(
                messageId,
                tokenContext.UserId,
                tokenContext.ConnectionId,
                request.Text,
                ct);

            return Ok(_messageMapper.ToResponse(message));
        }

        [Authorize]
        [HttpPatch("status")]
        public async Task<IActionResult> EditStatus(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            [FromBody] EditMessageStatusRequest request,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            await _messageOrchestrator.EditStatusAsync(
                request.Ids,
                request.ChatId,
                tokenContext.UserId,
                tokenContext.ConnectionId,
                request.Status,
                ct);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> Delete(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            [FromRoute] Guid messageId,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            await _messageService.DeleteMessage(
                messageId,
                tokenContext.UserId,
                ct);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{messageId}/{fileId}")]
        public async Task<IActionResult> DeleteFile(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            [FromRoute] Guid messageId,
            [FromRoute] Guid fileId,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            await _messageService.DeleteFile(
                messageId,
                fileId,
                tokenContext.UserId,
                ct);

            return Ok();
        }
    }
}