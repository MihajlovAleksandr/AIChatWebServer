using AIChatWebServer.Models.Payment;
using AIChatWebServer.Services.Context.Implementations;
using AIChatWebServer.Services.Implementations.Payments;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class StripeWebhookController(
        IConfiguration config,
        IPurchaseService purchaseService,
        IUserPremiumService userPremiumService,
        ILogger<StripeWebhookController> logger) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly IPurchaseService _purchaseService = purchaseService;
        private readonly IUserPremiumService _userPremiumService = userPremiumService;
        private readonly ILogger<StripeWebhookController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> Handle(CancellationToken ct)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _config["Stripe:WebhookSecret"]
                );
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe signature validation failed");
                return BadRequest();
            }

            _logger.LogWarning("Stripe event type: {Type}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "invoice.payment_succeeded":
                    await HandleInvoicePaymentSucceeded(stripeEvent, ct);
                    break;

                case "customer.subscription.deleted":
                    await HandleSubscriptionDeleted(stripeEvent, ct);
                    break;

                default:
                    _logger.LogWarning("Unhandled Stripe event type: {Type}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }

        private async Task HandleInvoicePaymentSucceeded(Event stripeEvent, CancellationToken ct)
        {
            if (stripeEvent.Data.Object is not Invoice invoice)
                return;

            var line = invoice.Lines?.Data?.FirstOrDefault();

            string? paymentIdStr = null;

            if (line?.Metadata != null &&
                line.Metadata.TryGetValue("paymentId", out var pid))
            {
                paymentIdStr = pid;
            }

            string? subscriptionId = invoice?
                .Parent?
                .SubscriptionDetails?
                .Subscription?.Id;

            if (string.IsNullOrEmpty(subscriptionId))
            {
                subscriptionId = line?
                    .Parent?
                    .SubscriptionItemDetails?
                    .Subscription;
            }

            var billingReason = invoice.BillingReason;

            _logger.LogWarning(
                "Invoice {InvoiceId}: billingReason={Reason}, paymentId={PaymentId}, subscriptionId={SubId}",
                invoice.Id,
                billingReason,
                paymentIdStr,
                subscriptionId);

            if (subscriptionId == null)
                throw new ArgumentException();

            SubscriptionPaymentData subscriptionPaymentData = new SubscriptionPaymentData(subscriptionId, true);

            if (billingReason == "subscription_create" &&
                !string.IsNullOrEmpty(paymentIdStr) &&
                Guid.TryParse(paymentIdStr, out var paymentId))
            {
                _logger.LogWarning("First subscription payment detected");

                await _purchaseService.ConfirmPaymentAsync(
                    paymentId,
                    invoice.Id,
                    subscriptionPaymentData,
                    ct);

                return;
            }

            if (billingReason == "subscription_cycle" &&
                !string.IsNullOrEmpty(subscriptionId))
            {
                var priceId = line?
                    .Pricing?
                    .PriceDetails?
                    .PriceId;

                if (string.IsNullOrEmpty(priceId))
                    return;

                var amount = invoice.AmountPaid / 100m;
                var currency = invoice.Currency;

                _logger.LogWarning("Subscription renewal detected");

                await _purchaseService.ProcessSubscriptionRenewalAsync(
                    subscriptionId,
                    invoice.Id,
                    priceId,
                    amount,
                    currency,
                    ct);

                return;
            }

            if (!string.IsNullOrEmpty(paymentIdStr) &&
                Guid.TryParse(paymentIdStr, out var fallbackPaymentId))
            {
                _logger.LogWarning("Fallback to ConfirmPayment");

                await _purchaseService.ConfirmPaymentAsync(
                    fallbackPaymentId,
                    invoice.Id,
                    new OneTimePaymentData(),
                    ct);

                return;
            }

            _logger.LogWarning(
                "Unhandled invoice.payment_succeeded. InvoiceId={InvoiceId}, Reason={Reason}",
                invoice.Id,
                billingReason);
        }

        private async Task HandleSubscriptionDeleted(Event stripeEvent, CancellationToken ct)
        {
            if (stripeEvent.Data.Object is not Subscription subscription)
                return;

            _logger.LogWarning("Subscription deleted: {Id}", subscription.Id);

            var subscriptionId = subscription.Id;

            if (string.IsNullOrEmpty(subscriptionId))
                throw new ArgumentException();

            await _userPremiumService.CancelAutoRenew(subscriptionId, ct);
        }
    }
}