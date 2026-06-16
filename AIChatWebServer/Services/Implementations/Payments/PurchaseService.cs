using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;
using AIChatWebServer.Services.Interfaces.Users;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public sealed class PurchaseService(
        IUnitOfWorkFactory unitOfWorkFactory,
        IProductRepository productRepository,
        IUserService userService,
        IProductAccessFilter filter,
        IPaymentRepository paymentRepository,
        IPaymentItemRepository paymentItemRepository,
        IUserPremiumRepository userPremiumRepository,
        IEnumerable<IPurchaseHandler> handlers) : IPurchaseService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IPaymentRepository _paymentRepository = paymentRepository;
        private readonly IProductAccessFilter _filter = filter;
        private readonly IUserService _userService = userService;
        private readonly IPaymentItemRepository _paymentItemRepository = paymentItemRepository;
        private readonly IUserPremiumRepository _userPremiumRepository = userPremiumRepository;
        private readonly IEnumerable<IPurchaseHandler> _handlers = handlers;

        public async Task<List<Product>> GetAvailableProductsAsync(
            Guid userId,
            string? type,
            CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(userId, ct);
            var list = type == null
                ? await _productRepository.GetActiveAsync(user.RegionCode, ct)
                : await _productRepository.GetByTypeAsync(type, user.RegionCode, true, ct);

            var tasks = list.Select(async item => new
            {
                Item = item,
                ShouldInclude = await _filter.ShouldInclude(item, userId, ct)
            });

            var results = await Task.WhenAll(tasks);
            return results.Where(x => x.ShouldInclude).Select(x => x.Item).ToList();

        }

        public async Task<Product> GetProductAsync(
            Guid productId,
            Guid userId,
            CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(userId, ct);

            var product = await _productRepository.GetByIdAsync(
                productId,
                user.RegionCode,
                ct);

            if (product == null || !product.IsActive)
                throw new ArgumentException(
                    $"Product {productId} not found or inactive.");

            return product;
        }

        public async Task<(Guid paymentId, decimal amount, string currency, List<PaymentItem> items)> CreatePaymentAsync(
            Guid userId,
            List<(Guid productId, int quantity)> items,
            CancellationToken ct)
        {
            if (items.Count == 0)
                throw new ArgumentException("Items cannot be empty.");

            var user = await _userService.GetByIdAsync(userId, ct);

            await using var uow = await _unitOfWorkFactory.CreateAsync(ct);

            var productRepo = uow.WithTransaction(_productRepository);
            var paymentRepo = uow.WithTransaction(_paymentRepository);
            var itemRepo = uow.WithTransaction(_paymentItemRepository);

            var products = new Dictionary<Guid, Product>();

            foreach (var (productId, _) in items)
            {
                var product = await productRepo.GetByIdAsync(
                    productId,
                    user.RegionCode,
                    ct)
                    ?? throw new ArgumentException(
                        $"Invalid product {productId}");

                if (!product.IsActive)
                    throw new ArgumentException(
                        $"Inactive product {productId}");

                products[productId] = product;
            }

            var currency = products.Values.First().Currency;

            var total = items.Sum(i =>
            {
                var p = products[i.productId];

                if (p.Currency != currency)
                    throw new InvalidOperationException(
                        "Mixed currencies are not supported.");

                return p.Price * i.quantity;
            });

            var paymentId = Guid.NewGuid();

            var payment = new Payment
            {
                Id = paymentId,
                TransactionId = null,
                StripeChargeId = null,
                StripeInvoiceUrl = null,
                UserId = userId,
                Amount = total,
                Currency = currency,
                Status = PaymentStatuses.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await paymentRepo.CreateAsync(payment, ct);

            var createdItems = new List<PaymentItem>();

            foreach (var (productId, quantity) in items)
            {
                var product = products[productId];

                var paymentItem = new PaymentItem
                {
                    Id = Guid.NewGuid(),
                    PaymentId = paymentId,
                    Product = product,
                    Quantity = quantity,
                    Price = product.Price
                };

                await itemRepo.CreateAsync(paymentItem, ct);

                createdItems.Add(paymentItem);
            }

            await uow.CommitAsync(ct);

            return (paymentId, total, currency, createdItems);
        }

        public async Task ConfirmPaymentAsync(
            Guid paymentId,
            string invoiceId,
            string? stripeChargeId,
            string? stripeInvoiceUrl,
            PaymentData data,
            CancellationToken ct)
        {
            await using var uow = await _unitOfWorkFactory.CreateAsync(ct);

            var paymentRepo = uow.WithTransaction(_paymentRepository);
            var itemRepo = uow.WithTransaction(_paymentItemRepository);

            var payment = await paymentRepo.GetByIdAsync(paymentId, ct)
                ?? throw new ArgumentException("Payment not found");

            if (payment.TransactionId == invoiceId)
                return;

            if (payment.Status == PaymentStatuses.Confirmed)
                return;

            var user = await _userService.GetByIdAsync(
                payment.UserId,
                ct);

            var items = await itemRepo.GetByPaymentAsync(
                paymentId,
                user.RegionCode,
                ct);

            foreach (var item in items)
            {
                var handler = _handlers.FirstOrDefault(
                    h => h.CanHandle(item.Product, data))
                    ?? throw new InvalidOperationException(
                        $"No handler for {item.Product.Type}");

                var transactionalHandler = uow.WithTransaction(handler);

                await transactionalHandler.ApplyAsync(
                    payment.UserId,
                    item,
                    data,
                    ct);
            }

            await paymentRepo.ConfirmAsync(
                paymentId,
                invoiceId,
                stripeChargeId,
                stripeInvoiceUrl,
                ct);

            await uow.CommitAsync(ct);
        }

        public async Task UpdateReceiptUrlAsync(
            string stripeChargeId,
            string stripeInvoiceUrl,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(stripeChargeId))
                throw new ArgumentException(
                    "Stripe charge id is required.",
                    nameof(stripeChargeId));

            if (string.IsNullOrWhiteSpace(stripeInvoiceUrl))
                throw new ArgumentException(
                    "Stripe invoice url is required.",
                    nameof(stripeInvoiceUrl));

            var payment = await _paymentRepository.GetByStripeChargeIdAsync(
                stripeChargeId,
                ct);

            if (payment == null)
                return;

            if (!string.IsNullOrWhiteSpace(payment.StripeInvoiceUrl))
                return;

            await _paymentRepository.UpdateReceiptUrlAsync(
                stripeChargeId,
                stripeInvoiceUrl,
                ct);
        }

        public async Task ProcessSubscriptionRenewalAsync(
            string subscriptionId,
            string invoiceId,
            string stripeInvoiceUrl,
            string priceId,
            decimal amount,
            string currency,
            PaymentData paymentData,
            CancellationToken ct)
        {
            await using var uow = await _unitOfWorkFactory.CreateAsync(ct);

            var paymentRepo = uow.WithTransaction(_paymentRepository);
            var itemRepo = uow.WithTransaction(_paymentItemRepository);
            var productRepo = uow.WithTransaction(_productRepository);
            var premiumRepo = uow.WithTransaction(_userPremiumRepository);

            if (await paymentRepo.ExistsByTransactionId(invoiceId, ct))
                return;

            var userId = await premiumRepo.GetUserIdBySubscriptionIdAsync(
                subscriptionId,
                ct)
                ?? throw new InvalidOperationException(
                    "Subscription not found");

            var user = await _userService.GetByIdAsync(userId, ct);

            var product = await productRepo.GetByStripePriceIdAsync(
                priceId,
                user.RegionCode,
                ct)
                ?? throw new ArgumentException(
                    $"Product not found for priceId {priceId}");

            if (currency.ToLowerInvariant()
                != product.Currency.ToLowerInvariant())
            {
                throw new InvalidOperationException(
                    "Currency mismatch");
            }

            var paymentId = Guid.NewGuid();

            var payment = new Payment
            {
                Id = paymentId,
                TransactionId = invoiceId,
                StripeChargeId = null,
                StripeInvoiceUrl = stripeInvoiceUrl,
                UserId = userId,
                Amount = amount,
                Currency = currency,
                Status = PaymentStatuses.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            await paymentRepo.CreateAsync(payment, ct);

            var paymentItem = new PaymentItem
            {
                Id = Guid.NewGuid(),
                PaymentId = paymentId,
                Product = product,
                Quantity = 1,
                Price = product.Price
            };

            await itemRepo.CreateAsync(paymentItem, ct);

            var handler = _handlers.First(
                h => h.CanHandle(product, paymentData));

            var transactionalHandler = uow.WithTransaction(handler);

            await transactionalHandler.ApplyAsync(
                userId,
                paymentItem,
                paymentData,
                ct);

            await uow.CommitAsync(ct);
        }

        public Task<List<Payment>> GetUserPaymentsAsync(
            Guid userId,
            CancellationToken ct)
        {
            return _paymentRepository.GetHistoryAsync(userId, ct);
        }

        public async Task<(Payment payment, List<PaymentItem> items)> GetPaymentDetailsAsync(
            Guid paymentId,
            CancellationToken ct)
        {
            var payment = await _paymentRepository.GetByIdAsync(
                paymentId,
                ct)
                ?? throw new ArgumentException(
                    "Payment not found");

            var user = await _userService.GetByIdAsync(
                payment.UserId,
                ct);

            var items = await _paymentItemRepository.GetByPaymentAsync(
                paymentId,
                user.RegionCode,
                ct);

            return (payment, items);
        }

        public async Task FailSubscriptionRenewalAsync(
            string subscriptionId,
            string invoiceId,
            string? stripeChargeId,
            string? stripeInvoiceUrl,
            CancellationToken ct)
        {
            await using var uow = await _unitOfWorkFactory.CreateAsync(ct);

            var paymentRepo = uow.WithTransaction(_paymentRepository);
            var premiumRepo = uow.WithTransaction(_userPremiumRepository);

            if (await paymentRepo.ExistsByTransactionId(invoiceId, ct))
                return;

            var userId = await premiumRepo.GetUserIdBySubscriptionIdAsync(
                subscriptionId,
                ct);

            if (userId == null)
            {
                throw new InvalidOperationException(
                    "Subscription not found");
            }

            var failedTransactionId = BuildFailedTransactionId(invoiceId);

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                TransactionId = failedTransactionId,
                StripeChargeId = stripeChargeId,
                StripeInvoiceUrl = stripeInvoiceUrl,
                UserId = userId.Value,
                Amount = 0,
                Currency = string.Empty,
                Status = PaymentStatuses.Failed,
                CreatedAt = DateTime.UtcNow
            };

            await paymentRepo.CreateAsync(payment, ct);

            await paymentRepo.FailAsync(
                payment.Id,
                failedTransactionId,
                stripeChargeId,
                stripeInvoiceUrl,
                ct);

            await uow.CommitAsync(ct);
        }

        public async Task FailPaymentAsync(
            Guid paymentId,
            string? invoiceId,
            string? stripeChargeId,
            string? stripeInvoiceUrl,
            CancellationToken ct)
        {
            await using var uow = await _unitOfWorkFactory.CreateAsync(ct);

            var paymentRepo = uow.WithTransaction(_paymentRepository);

            var payment = await paymentRepo.GetByIdAsync(paymentId, ct)
                ?? throw new ArgumentException("Payment not found");

            if (payment.Status == PaymentStatuses.Failed)
                return;

            if (payment.Status == PaymentStatuses.Confirmed)
                return;

            var failedTransactionId = invoiceId is null
                ? $"FAILED_UNKNOWN_{DateTime.UtcNow:yyyyMMddHHmmssfff}"
                : BuildFailedTransactionId(invoiceId);

            await paymentRepo.FailAsync(
                paymentId,
                failedTransactionId,
                stripeChargeId,
                stripeInvoiceUrl,
                ct);

            await uow.CommitAsync(ct);
        }

        public async Task<(Payment payment, List<PaymentItem> items)> GetPremiumPaymentDetailsAsync(
            Guid premiumId,
            CancellationToken ct)
        {
            var payment = await _paymentRepository.GetByPremiumIdAsync(
                premiumId,
                ct)
                ?? throw new ArgumentException(
                    "Payment not found");

            var user = await _userService.GetByIdAsync(
                payment.UserId,
                ct);

            var items = await _paymentItemRepository.GetByPaymentAsync(
                payment.Id,
                user.RegionCode,
                ct);

            return (payment, items);
        }
        private static string BuildFailedTransactionId(string invoiceId)
        {
            return $"FAILED_{invoiceId}_{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}