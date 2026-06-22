using AIChatWebServer.DTO.Request;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Services.Interfaces.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController(IConnectionValidator connectionValidator, IFileService fileService) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IFileService _fileService = fileService;

        [Authorize]
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> UploadAsync(
            IFormFile file,
            [FromForm] UploadFileRequest request,
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device, ct);

            Guid fileId = await _fileService.UploadAsync(request.SessionId, request.FileId, file, request.FileType, tokenContext.UserId, ct);

            return Ok(fileId);
        }

        [Authorize]
        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid fileId,
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device, ct);

            await _fileService.DeleteAsync(fileId, ct);

            return Ok();
        }

        [Authorize]
        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetAsync(
           [FromRoute] Guid fileId,
           [FromServices] IWorkTokenContext tokenContext,
           [FromServices] IClientContext clientContext,
           CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            var result = await _fileService.GetById(fileId, ct);

            Response.Headers["X-File-Type"] = result.FileType.ToString();

            return File(
                result.Stream,
                result.ContentType,
                result.FileName);
        }
    }
}
