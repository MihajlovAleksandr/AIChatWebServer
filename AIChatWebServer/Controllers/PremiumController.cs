using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Services.Interfaces.Users;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/user/premium")]
    public class PremiumController(
        IResponseMapper<IReadOnlyCollection<UserPremium>, PremiumInfoResponse> premiumInfoMapper,
        IResponseMapper<UserPremium, UserPremiumResponse> premiumMapper, 
        ICollectionResponseMapper<UserPremium, UserPremiumResponse> premiumCollectionMapper,
        IConnectionValidator connectionValidator, IUserPremiumService premiumService) : ControllerBase
    {
        private readonly IResponseMapper<IReadOnlyCollection<UserPremium>, PremiumInfoResponse> _premiumInfoMapper = premiumInfoMapper;
        private readonly IResponseMapper<UserPremium, UserPremiumResponse> _premiumMapper = premiumMapper;
        private readonly ICollectionResponseMapper<UserPremium, UserPremiumResponse> _premiumCollectionMapper = premiumCollectionMapper;
        private readonly IUserPremiumService _premiumService = premiumService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken cancellationToken)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, cancellationToken);

            return Ok(_premiumInfoMapper.ToResponse(await _premiumService.GetHistoryAsync(tokenContext.UserId, cancellationToken)));
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken cancellationToken)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, cancellationToken);

            return Ok(_premiumCollectionMapper.ToResponse(await _premiumService.GetHistoryAsync(tokenContext.UserId, cancellationToken)));
        }

        [Authorize]
        [HttpGet("is-active")]
        public async Task<IActionResult> IsActive(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken cancellationToken)
        {
            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                cancellationToken);

            var premium = await _premiumService.GetActiveAsync(
                tokenContext.UserId,
                cancellationToken);

            return Ok(premium != null);
        }

        [Authorize]
        [HttpGet("auto-renew")]
        public async Task<IActionResult> GetAutoRenew(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken cancellationToken)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, cancellationToken);

            UserPremium? premium = await _premiumService.GetAutoRenewAsync(tokenContext.UserId, cancellationToken);

            if (premium == null) return NoContent();

            return Ok(_premiumMapper.ToResponse(premium));
        }


        [Authorize]
        [HttpGet("last")]
        public async Task<IActionResult> GetLast(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken cancellationToken)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, cancellationToken);

            UserPremium? premium = await _premiumService.GetLastAsync(tokenContext.UserId, cancellationToken);

            if (premium == null) return NoContent();

            return Ok(_premiumMapper.ToResponse(premium));
        }

    }
}
