using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions.Implementations.Payment;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Implementations;
using AIChatWebServer.Services.Implementations.Payments;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Services.Interfaces.Payments;
using AIChatWebServer.Services.Interfaces.Users;
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
        IResponseMapper<Product, ProductResponse> productMapper,
        IUserPremiumService userPremiumService,
        ICollectionResponseMapper<Payment, PaymentInfoResponse> paymentInfoMapper,
        IResponseMapper<(Payment payment, List<PaymentItem> items), PaymentResponse> paymentMapper)
        : ControllerBase
    {
        private readonly IPurchaseService _purchaseService = purchaseService;
        private readonly IServerValidator _serverValidator = serverValidator;
        private readonly IUserPremiumService _userPremiumService = userPremiumService;
        private readonly ICollectionResponseMapper<Product, ProductResponse> _productCollectionMapper = productCollectionMapper;
        private readonly IResponseMapper<Product, ProductResponse> _productMapper = productMapper;
        private readonly ICollectionResponseMapper<Payment, PaymentInfoResponse> _paymentInfoMapper = paymentInfoMapper;
        private readonly IResponseMapper<(Payment payment, List<PaymentItem> items), PaymentResponse> _paymentMapper = paymentMapper;

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

            if (request.Type == PaymentType.Subscription)
            {
                if (request.Items.Count != 1)
                    throw new InvalidSubscriptionPaymentItemsException(request.Items.Count);

                UserPremium? userPremium =
                    await _userPremiumService.GetActiveAsync(request.UserId, ct);

                if (userPremium != null && userPremium.IsAutoRenew)
                    throw new UserAlreadyHasAutoRenewSubscriptionException(request.UserId);
            }

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

            return Ok(_paymentInfoMapper.ToResponse(payments));
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

            return Ok(_paymentMapper.ToResponse(await _purchaseService
                .GetPaymentDetailsAsync(id, ct)));
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

            await _purchaseService.ConfirmPaymentAsync(id, request.TransactionId, request.StripeChargeId, request.StripeInvoiceUrl, new OneTimePaymentData(), ct);

            return Ok();
        }
    }
}