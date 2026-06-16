using AIChatWebServer.Models.Payment;
using AIChatWebServer.Services.Interfaces.Payments;
using AIChatWebServer.Services.Interfaces.Users;
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
        ISubscriptionPaymentDataCreator creator,
        ILogger<StripeWebhookController> logger) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly IPurchaseService _purchaseService = purchaseService;
        private readonly IUserPremiumService _userPremiumService = userPremiumService;
        private readonly ISubscriptionPaymentDataCreator _creator = creator;
        private readonly ILogger<StripeWebhookController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> Handle(CancellationToken ct)
        {
            ct = CancellationToken.None;

            var json = await new StreamReader(
                HttpContext.Request.Body)
                .ReadToEndAsync(ct);

            _logger.LogInformation(json);

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _config["Stripe:WebhookSecret"]);
            }
            catch (StripeException e)
            {
                _logger.LogError(
                    e,
                    "Stripe signature validation failed");

                return BadRequest();
            }

            _logger.LogWarning(
                "Stripe event type: {Type}",
                stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "invoice.payment_succeeded":
                    await HandleInvoicePaymentSucceeded(
                        stripeEvent,
                        ct);
                    break;

                case "invoice.payment_failed":
                    await HandleInvoicePaymentFailed(
                        stripeEvent,
                        ct);
                    break;

                case "customer.subscription.deleted":
                    await HandleSubscriptionDeleted(
                        stripeEvent,
                        ct);
                    break;

                case "payment_intent.succeeded":
                    await HandlePaymentIntentSucceeded(
                        stripeEvent,
                        ct);
                    break;
                case "payment_intent.payment_failed":
                    await HandlePaymentIntentFailed(
                        stripeEvent,
                        ct);
                    break;

                case "charge.updated":
                    await HandleChargeUpdated(
                        stripeEvent,
                        ct);
                    break;

                default:
                    _logger.LogWarning(
                        "Unhandled Stripe event type: {Type}",
                        stripeEvent.Type);
                    break;
            }

            return Ok();
        }

        private async Task HandleInvoicePaymentSucceeded(
            Event stripeEvent,
            CancellationToken ct)
        {
            if (stripeEvent.Data.Object is not Invoice invoice)
                return;

            var line = invoice.Lines?.Data?.FirstOrDefault();

            string? paymentIdStr = null;

            if (line?.Metadata != null &&
                line.Metadata.TryGetValue(
                    "paymentId",
                    out var pid))
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
                throw new ArgumentException(
                    "Subscription id is null.");

            var invoiceUrl = invoice.HostedInvoiceUrl;
            if (subscriptionId == null)
                throw new ArgumentException(
                    "Subscription id is null.");

            if (billingReason == "subscription_create" &&
                !string.IsNullOrEmpty(paymentIdStr) &&
                Guid.TryParse(paymentIdStr, out var paymentId))
            {
                _logger.LogWarning(
                    "First subscription payment detected");
                
                await _purchaseService.ConfirmPaymentAsync(
                    paymentId,
                    invoice.Id,
                    null,
                    invoiceUrl,
                    await _creator.Create(subscriptionId, ct),
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

                _logger.LogWarning(
                    "Subscription renewal detected");

                await _purchaseService.ProcessSubscriptionRenewalAsync(
                    subscriptionId,
                    invoice.Id,
                    invoiceUrl,
                    priceId,
                    amount,
                    currency,
                    await _creator.Create(subscriptionId, ct),
                    ct);

                return;
            }

            if (!string.IsNullOrEmpty(paymentIdStr) &&
                Guid.TryParse(paymentIdStr, out var fallbackPaymentId))
            {
                _logger.LogWarning(
                    "Fallback to ConfirmPayment");

                await _purchaseService.ConfirmPaymentAsync(
                    fallbackPaymentId,
                    invoice.Id,
                    null,
                    invoiceUrl,
                    new OneTimePaymentData(),
                    ct);

                return;
            }

            _logger.LogWarning(
                "Unhandled invoice.payment_succeeded. InvoiceId={InvoiceId}, Reason={Reason}",
                invoice.Id,
                billingReason);
        }

        private async Task HandleSubscriptionDeleted(
            Event stripeEvent,
            CancellationToken ct)
        {
            if (stripeEvent.Data.Object is not Subscription subscription)
                return;

            _logger.LogWarning(
                "Subscription deleted: {Id}",
                subscription.Id);

            var subscriptionId = subscription.Id;

            if (string.IsNullOrEmpty(subscriptionId))
                throw new ArgumentException(
                    "Subscription id is null.");

            await _userPremiumService.CancelAutoRenew(
                subscriptionId,
                ct);
        }

        private async Task HandlePaymentIntentSucceeded(
            Event stripeEvent,
            CancellationToken ct)
        {
            if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
                return;

            _logger.LogWarning(
                "PaymentIntent succeeded: {PaymentIntentId}",
                paymentIntent.Id);

            if (paymentIntent.Metadata == null ||
                !paymentIntent.Metadata.TryGetValue(
                    "paymentId",
                    out var paymentIdStr))
            {
                _logger.LogWarning(
                    "PaymentIntent {PaymentIntentId} has no paymentId metadata",
                    paymentIntent.Id);

                return;
            }

            if (!Guid.TryParse(paymentIdStr, out var paymentId))
            {
                _logger.LogWarning(
                    "Invalid paymentId metadata: {PaymentId}",
                    paymentIdStr);

                return;
            }

            await _purchaseService.ConfirmPaymentAsync(
                paymentId,
                paymentIntent.Id,
                paymentIntent.LatestChargeId,
                null,
                new OneTimePaymentData(),
                ct);

            _logger.LogWarning(
                "One-time payment confirmed. PaymentId={PaymentId}, StripePaymentIntentId={StripePaymentIntentId}, StripeChargeId={StripeChargeId}",
                paymentId,
                paymentIntent.Id,
                paymentIntent.LatestChargeId);
        }

        private async Task HandleChargeUpdated(
            Event stripeEvent,
            CancellationToken ct)
        {
            if (stripeEvent.Data.Object is not Charge charge)
                return;

            _logger.LogWarning(
                "Charge updated: {ChargeId}",
                charge.Id);

            if (string.IsNullOrWhiteSpace(charge.ReceiptUrl))
            {
                _logger.LogWarning(
                    "Charge {ChargeId} has no receipt url",
                    charge.Id);

                return;
            }

            await _purchaseService.UpdateReceiptUrlAsync(
                charge.Id,
                charge.ReceiptUrl,
                ct);

            _logger.LogWarning(
                "Receipt url updated for charge {ChargeId}",
                charge.Id);
        }

        private async Task HandleInvoicePaymentFailed(
     Event stripeEvent,
     CancellationToken ct)
        {
            if (stripeEvent.Data.Object is not Invoice invoice)
                return;

            _logger.LogWarning(
                "Invoice payment failed: {InvoiceId}",
                invoice.Id);

            var billingReason = invoice.BillingReason;

            var line = invoice.Lines?.Data?.FirstOrDefault();

            string? paymentIdStr = null;

            if (line?.Metadata != null &&
                line.Metadata.TryGetValue(
                    "paymentId",
                    out var pid))
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

            var invoiceUrl = invoice.HostedInvoiceUrl;

            switch (billingReason)
            {
                case "subscription_create":
                    _logger.LogWarning(
                        "Subscription first payment failed. InvoiceId={InvoiceId}",
                        invoice.Id);

                    if (!string.IsNullOrEmpty(paymentIdStr) &&
                        Guid.TryParse(paymentIdStr, out var paymentId))
                    {
                        await _purchaseService.FailPaymentAsync(
                            paymentId,
                            invoice.Id,
                            null,
                            invoiceUrl,
                            ct);

                        _logger.LogWarning(
                            "First subscription payment marked as failed. PaymentId={PaymentId}",
                            paymentId);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Unable to resolve paymentId for failed subscription_create invoice. InvoiceId={InvoiceId}",
                            invoice.Id);
                    }

                    break;

                case "subscription_cycle":
                    _logger.LogWarning(
                        "Subscription renewal payment failed. InvoiceId={InvoiceId}",
                        invoice.Id);

                    if (string.IsNullOrWhiteSpace(subscriptionId))
                    {
                        _logger.LogWarning(
                            "Subscription id is missing for failed renewal. InvoiceId={InvoiceId}",
                            invoice.Id);

                        return;
                    }

                    await _purchaseService.FailSubscriptionRenewalAsync(
                        subscriptionId,
                        invoice.Id,
                        null,
                        invoiceUrl,
                        ct);

                    _logger.LogWarning(
                        "Subscription renewal marked as failed. SubscriptionId={SubscriptionId}",
                        subscriptionId);

                    break;

                default:
                    _logger.LogWarning(
                        "Unhandled invoice.payment_failed reason. InvoiceId={InvoiceId}, Reason={Reason}",
                        invoice.Id,
                        billingReason);

                    break;
            }
        }

        private async Task HandlePaymentIntentFailed(
            Event stripeEvent,
            CancellationToken ct)
        {
            if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
                return;

            _logger.LogWarning(
                "PaymentIntent failed: {PaymentIntentId}",
                paymentIntent.Id);

            if (paymentIntent.Metadata == null ||
                !paymentIntent.Metadata.TryGetValue(
                    "paymentId",
                    out var paymentIdStr))
            {
                _logger.LogWarning(
                    "PaymentIntent {PaymentIntentId} has no paymentId metadata",
                    paymentIntent.Id);

                return;
            }

            if (!Guid.TryParse(paymentIdStr, out var paymentId))
            {
                _logger.LogWarning(
                    "Invalid paymentId metadata: {PaymentId}",
                    paymentIdStr);

                return;
            }

            await _purchaseService.FailPaymentAsync(
                paymentId,
                paymentIntent.Id,
                paymentIntent.LatestChargeId,
                null,
                ct);

            _logger.LogWarning(
                "One-time payment marked as failed. PaymentId={PaymentId}, StripePaymentIntentId={StripePaymentIntentId}",
                paymentId,
                paymentIntent.Id);
        }
    }
}