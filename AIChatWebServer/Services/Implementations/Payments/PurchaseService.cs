using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public sealed class PurchaseService(
        IUnitOfWorkFactory unitOfWorkFactory,
        IProductRepository productRepository,
        IUserService userService,
        IPaymentRepository paymentRepository,
        IPaymentItemRepository paymentItemRepository,
        IUserPremiumRepository userPremiumRepository,
        IEnumerable<IPurchaseHandler> handlers) : IPurchaseService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IPaymentRepository _paymentRepository = paymentRepository;
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

            return type == null
                ? await _productRepository.GetActiveAsync(user.RegionCode, ct)
                : await _productRepository.GetByTypeAsync(type, user.RegionCode, true, ct);
        }

        public async Task<Product> GetProductAsync(
            Guid productId,
            Guid userId,
            CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(userId, ct);

            var product = await _productRepository.GetByIdAsync(productId, user.RegionCode, ct);

            if (product == null || !product.IsActive)
                throw new ArgumentException($"Product {productId} not found or inactive.");

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
                var product = await productRepo.GetByIdAsync(productId, user.RegionCode, ct)
                    ?? throw new ArgumentException($"Invalid product {productId}");

                if (!product.IsActive)
                    throw new ArgumentException($"Inactive product {productId}");

                products[productId] = product;
            }

            var currency = products.Values.First().Currency;

            var total = items.Sum(i =>
            {
                var p = products[i.productId];
                if (p.Currency != currency)
                    throw new InvalidOperationException("Mixed currencies are not supported.");

                return p.Price * i.quantity;
            });

            var paymentId = Guid.NewGuid();

            var payment = new Payment
            {
                Id = paymentId,
                TransactionId = null,
                UserId = userId,
                Amount = total,
                Currency = currency,
                Status = "PENDING",
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

        public async Task ConfirmPaymentAsync(Guid paymentId, string invoiceId, PaymentData data, CancellationToken ct)
        {
            await using var uow = await _unitOfWorkFactory.CreateAsync(ct);

            var paymentRepo = uow.WithTransaction(_paymentRepository);
            var itemRepo = uow.WithTransaction(_paymentItemRepository);

            var payment = await paymentRepo.GetByIdAsync(paymentId, ct)
                ?? throw new ArgumentException("Payment not found");

            if (payment.TransactionId == invoiceId)
                return;

            if (payment.Status == "CONFIRMED")
                return;

            var user = await _userService.GetByIdAsync(payment.UserId, ct);

            var items = await itemRepo.GetByPaymentAsync(paymentId, user.RegionCode, ct);

            foreach (var item in items)
            {
                var handler = _handlers.FirstOrDefault(h => h.CanHandle(item.Product, data))
                    ?? throw new InvalidOperationException($"No handler for {item.Product.Type}");

                await handler.ApplyAsync(payment.UserId, item, data, uow, ct);
            }

            await paymentRepo.ConfirmAsync(paymentId, invoiceId, ct);

            await uow.CommitAsync(ct);
        }

        public async Task ProcessSubscriptionRenewalAsync(
            string subscriptionId,
            string invoiceId,
            string priceId,
            decimal amount,
            string currency,
            CancellationToken ct)
        {
            await using var uow = await _unitOfWorkFactory.CreateAsync(ct);

            var paymentRepo = uow.WithTransaction(_paymentRepository);
            var itemRepo = uow.WithTransaction(_paymentItemRepository);
            var productRepo = uow.WithTransaction(_productRepository);
            var premiumRepo = uow.WithTransaction(_userPremiumRepository);

            if (await paymentRepo.ExistsByTransactionId(invoiceId, ct))
                return;

            var userId = await premiumRepo.GetUserIdBySubscriptionIdAsync(subscriptionId, ct)
                ?? throw new InvalidOperationException("Subscription not found");

            var user = await _userService.GetByIdAsync(userId, ct);

            var product = await productRepo.GetByStripePriceIdAsync(priceId, user.RegionCode, ct)
                ?? throw new ArgumentException($"Product not found for priceId {priceId}");

            if (currency.ToLower() != product.Currency.ToLower())
                throw new InvalidOperationException("Currency mismatch");

            var paymentId = Guid.NewGuid();

            var payment = new Payment
            {
                Id = paymentId,
                TransactionId = invoiceId,
                UserId = userId,
                Amount = amount,
                Currency = currency,
                Status = "CONFIRMED",
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

            var data = new SubscriptionPaymentData(subscriptionId, false);

            var handler = _handlers.First(h => h.CanHandle(product, data));

            await handler.ApplyAsync(userId, paymentItem, data, uow, ct);

            await uow.CommitAsync(ct);
        }

        public Task<List<Payment>> GetUserPaymentsAsync(Guid userId, CancellationToken ct)
            => _paymentRepository.GetHistoryAsync(userId, ct);

        public async Task<(Payment payment, List<PaymentItem> items)> GetPaymentDetailsAsync(
            Guid paymentId,
            CancellationToken ct)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId, ct)
                ?? throw new ArgumentException("Payment not found");

            var user = await _userService.GetByIdAsync(payment.UserId, ct);

            var items = await _paymentItemRepository.GetByPaymentAsync(paymentId, user.RegionCode, ct);

            return (payment, items);
        }
    }
}