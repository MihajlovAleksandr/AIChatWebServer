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
    [Route("server/payments")]
    public sealed class ServerPaymentController(
        IPurchaseService purchaseService,
        IServerValidator serverValidator,
        ICollectionResponseMapper<Product, ProductResponse> productCollectionMapper,
        IResponseMapper<Product, ProductResponse> productMapper)
        : ControllerBase
    {
        private readonly IPurchaseService _purchaseService = purchaseService;
        private readonly IServerValidator _serverValidator = serverValidator;
        private readonly ICollectionResponseMapper<Product, ProductResponse> _productCollectionMapper = productCollectionMapper;
        private readonly IResponseMapper<Product, ProductResponse> _productMapper = productMapper;
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromBody] ServerCreatePaymentRequest request,
            [FromServices] IServerTokenContext serverTokenContext,
            CancellationToken ct)
        {
            await _serverValidator.Validate(
                serverTokenContext.UserId,
                serverTokenContext.Server,
                ct);

            var result = await _purchaseService.CreatePaymentAsync(
                request.UserId,
                request.Items.Select(i => (i.ProductId, i.Quantity)).ToList(),
                ct);

            return Ok(new CreatePaymentResponse(
                result.paymentId,
                result.amount,
                result.currency,
                result.items.Select(i => new PaymentItemResponse(
                    _productMapper.ToResponse(i.Product),
                    i.Quantity))));
        }

        [Authorize]
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserPayments(
            Guid userId,
            [FromServices] IServerTokenContext serverTokenContext,
            CancellationToken ct)
        {
            await _serverValidator.Validate(
                serverTokenContext.UserId,
                serverTokenContext.Server,
                ct);

            var payments = await _purchaseService.GetUserPaymentsAsync(
                userId,
                ct);

            return Ok(payments);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(
            Guid id,
            [FromServices] IServerTokenContext serverTokenContext,
            CancellationToken ct)
        {
            await _serverValidator.Validate(
                serverTokenContext.UserId,
                serverTokenContext.Server,
                ct);

            var (payment, items) = await _purchaseService
                .GetPaymentDetailsAsync(id, ct);

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
        [HttpGet("products/{userId:guid}")]
        public async Task<IActionResult> GetProducts(
            Guid userId,
            [FromQuery] string? type,
            [FromServices] IServerTokenContext serverTokenContext,
            CancellationToken ct)
        {
            await _serverValidator.Validate(
                serverTokenContext.UserId,
                serverTokenContext.Server,
                ct);

            var products = await _purchaseService.GetAvailableProductsAsync(
                userId,
                type,
                ct);

            return Ok(_productCollectionMapper.ToResponse(products));
        }

        [Authorize]
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmPayment(
            Guid id,
            [FromServices] IServerTokenContext serverTokenContext,
            [FromBody] ConfirmPaymentServerRequest request,
            CancellationToken ct)
        {
            await _serverValidator.Validate(
                serverTokenContext.UserId,
                serverTokenContext.Server,
                ct);

            await _purchaseService.ConfirmPaymentAsync(id, request.TransactionId, new OneTimePaymentData(), ct);

            return Ok();
        }
    }
}