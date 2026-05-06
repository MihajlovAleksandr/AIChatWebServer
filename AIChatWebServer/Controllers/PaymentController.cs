using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public sealed class PaymentController(
        IConnectionValidator connectionValidator,
        ICollectionResponseMapper<Product, ProductResponse> productCollectionMapper,
        IResponseMapper<Product, ProductResponse> productMapper,
        IPaymentDispatcher paymentDispatcher)
        : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly ICollectionResponseMapper<Product, ProductResponse> _productCollectionMapper = productCollectionMapper;
        private readonly IResponseMapper<Product, ProductResponse> _productMapper = productMapper;
        private readonly IPaymentDispatcher _paymentDispatcher = paymentDispatcher;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromBody] CreatePaymentRequest request,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);
            return Ok(await _paymentDispatcher.CreatePayment(
                request.Items.Select(i => (i.ProductId, i.Quantity)).ToList(),
                request.Type, workTokenContext.UserId,
                ct));
        }

        [Authorize]
        [HttpPost("cancel-subscription")]
        public async Task<IActionResult> CancelSubscription(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            await _paymentDispatcher.CancelSubscription(workTokenContext.UserId, ct);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyPayments(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            var payments = await _paymentDispatcher.GetMyPayments(
                workTokenContext.UserId,
                ct);

            return Ok(payments);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(
            Guid id,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            var (payment, items) = await _paymentDispatcher.GetPayment(id, ct);

            var response = new PaymentResponse(
                payment.Id,
                payment.Amount,
                payment.Currency,
                payment.Status,
                payment.CreatedAt,
                items.Select(i => new PaymentItemResponse(
                   _productMapper.ToResponse(i.Product),
                    i.Quantity)));

            return Ok(response);
        }


        [Authorize]
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? type,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            var products = await _paymentDispatcher.GetProducts(
                type,
                workTokenContext.UserId,
                ct);

            return Ok(_productCollectionMapper.ToResponse(products));
        }
    }
}