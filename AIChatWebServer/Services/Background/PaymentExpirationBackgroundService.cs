using AIChatWebServer.Repositories.Interfaces;

namespace AIChatWebServer.Services.Background
{
    public sealed class PaymentExpirationBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentExpirationBackgroundService> logger) : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);

        private static readonly TimeSpan PaymentLifetime = TimeSpan.FromMinutes(30);

        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<PaymentExpirationBackgroundService> _logger = logger;

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExpirePaymentsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while expiring payments");
                }

                await Task.Delay(
                    CheckInterval,
                    stoppingToken);
            }
        }

        private async Task ExpirePaymentsAsync(
            CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();

            var paymentRepository =
                scope.ServiceProvider
                    .GetRequiredService<IPaymentRepository>();

            var expiredBeforeUtc =
                DateTime.UtcNow - PaymentLifetime;

            await paymentRepository.ExpirePendingPaymentsAsync(
                expiredBeforeUtc,
                ct);
        }
    }
}