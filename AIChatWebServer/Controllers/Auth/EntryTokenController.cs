using AIChatWebServer.Models.Exceptions.Implementations.Auth.VerificationCode;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Tokens.Interfaces;
using AIChatWebServer.Utils.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/code")]
    public sealed class EntryTokenController(
        IConnectionService connectionService,
        IConnectionValidator connectionValidator,
        IEntryCodeService entryCodeService,
        IWorkTokenFactory workTokenFactory)
        : ControllerBase
    {
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IEntryCodeService _entryCodeService = entryCodeService;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;

        [Authorize]
        [HttpGet("generate")]
        public async Task<IActionResult> GenerateEntryCode(
            [FromServices] IWorkTokenContext workContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            if (clientContext.Device == null)
            {
                return Unauthorized(
                    ApiError.Create(CodeErrors.ContextInvalid));
            }
            await _connectionValidator.ValidateConnectionAsync(
                workContext.ConnectionId,
                workContext.UserId,
                clientContext.Device,
                ct);
            try
            {
                string code =
                    await _entryCodeService.GenerateAsync(
                        workContext.UserId,
                        ct);

                return Ok(code);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiError.Create(CodeErrors.GenerateFailed));
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEntryCode(
            [FromServices] IEntryTokenContext entryContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {

            if (string.IsNullOrWhiteSpace(clientContext.Device))
            {
                return BadRequest(
                    ApiError.Create(CodeErrors.DeviceMissing));
            }

            try
            {
                await _entryCodeService.VerifyAsync(
                    entryContext.UserId,
                    entryContext.Code,
                    ct);
            }
            catch (InvalidVerificationCodeException)
            {
                return Unauthorized(
                    ApiError.Create(CodeErrors.InvalidCode));
            }
            catch (VerificationCodeExpiredException)
            {
                return BadRequest(
                    ApiError.Create(CodeErrors.CodeExpired));
            }
            catch (VerificationCodeAttemptsExceededException)
            {
                return Forbid();
            }
            catch (VerificationCodeNotFoundException)
            {
                return NotFound(
                    ApiError.Create(CodeErrors.CodeNotFound));
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiError.Create(CodeErrors.VerificationFailed));
            }

            try
            {
                Guid connectionId =
                    await _connectionService.AddConnectionAsync(
                        clientContext.Device,
                        entryContext.UserId,
                        ct);

                return Ok(
                    _workTokenFactory.Create(
                        entryContext.UserId,
                        connectionId));
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiError.Create(CommonErrors.InternalError));
            }
        }
    }
}
