using Asp.Versioning;
using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Specifications.PaymentSpecifications;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Eventify.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IEmailService _emailService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly IConfiguration _configuration;

        public StripeWebhookController(
            IPaymentService paymentService,
            IRepository<Payment> paymentRepository,
            IEmailService emailService,
            IInvoiceService invoiceService,
            ILogger<StripeWebhookController> logger,
            IConfiguration configuration)
        {
            _paymentService = paymentService;
            _paymentRepository = paymentRepository;
            _emailService = emailService;
            _invoiceService = invoiceService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var stripeSignature = Request.Headers["Stripe-Signature"];

                // Validate webhook signature
                var validationResult = await _paymentService.ValidateWebhookSignatureAsync(json, stripeSignature!);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Invalid webhook signature received");
                    return BadRequest("Invalid signature");
                }

                // Parse the event
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _configuration["Stripe:WebhookSecret"]);

                _logger.LogInformation("Processing Stripe webhook: {EventType} - {EventId}",
                    stripeEvent.Type, stripeEvent.Id);

                // Handle different event types
                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        await HandlePaymentSucceeded(stripeEvent);
                        break;

                    case "payment_intent.payment_failed":
                        await HandlePaymentFailed(stripeEvent);
                        break;

                    case "payment_intent.canceled":
                        await HandlePaymentCanceled(stripeEvent);
                        break;

                    case "payment_intent.requires_action":
                        await HandlePaymentRequiresAction(stripeEvent);
                        break;

                    case "charge.dispute.created":
                        await HandleDisputeCreated(stripeEvent);
                        break;

                    case "invoice.payment_succeeded":
                        await HandleInvoicePaymentSucceeded(stripeEvent);
                        break;

                    case "customer.subscription.created":
                    case "customer.subscription.updated":
                    case "customer.subscription.deleted":
                        await HandleSubscriptionEvent(stripeEvent);
                        break;

                    default:
                        _logger.LogInformation("Unhandled webhook event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return Ok(new { received = true });
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook error: {Message}", ex.Message);
                return BadRequest($"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing webhook");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task HandlePaymentSucceeded(Event stripeEvent)
        {
            try
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent == null) return;

                _logger.LogInformation("Payment succeeded: {PaymentIntentId}", paymentIntent.Id);

                var paymentSpec = new PaymentByExternalIdSpecification(paymentIntent.Id);
                var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);

                if (payment != null)
                {
                    payment.Complete(paymentIntent.Id);
                    await _paymentRepository.UpdateAsync(payment);

                    if (payment.Booking != null)
                    {
                        payment.Booking.Confirm();
                    }

                    if (payment.Booking?.User != null)
                    {
                        await SendPaymentConfirmationEmail(payment);
                    }

                    var receiptResult = await _invoiceService.GenerateReceiptAsync(payment.Id);
                    if (receiptResult.IsSuccess)
                    {
                        _logger.LogInformation("Receipt generated for payment: {PaymentId}", payment.Id);
                    }

                    _logger.LogInformation("Payment processing completed for: {PaymentId}", payment.Id);
                }
                else
                {
                    _logger.LogWarning("Payment not found in database: {PaymentIntentId}", paymentIntent.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment succeeded webhook");
            }
        }

        private async Task HandlePaymentFailed(Event stripeEvent)
        {
            try
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent == null) return;

                _logger.LogInformation("Payment failed: {PaymentIntentId}", paymentIntent.Id);

                var paymentSpec = new PaymentByExternalIdSpecification(paymentIntent.Id);
                var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);

                if (payment != null)
                {
                    var failureReason = paymentIntent.LastPaymentError?.Message ?? "Payment failed";
                    payment.Fail(failureReason);
                    await _paymentRepository.UpdateAsync(payment);

                    // Send failure notification email
                    if (payment.Booking?.User != null)
                    {
                        await SendPaymentFailureEmail(payment, failureReason);
                    }

                    _logger.LogInformation("Payment failure processed for: {PaymentId}", payment.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment failed webhook");
            }
        }

        private async Task HandlePaymentCanceled(Event stripeEvent)
        {
            try
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent == null) return;

                _logger.LogInformation("Payment canceled: {PaymentIntentId}", paymentIntent.Id);

                var paymentSpec = new PaymentByExternalIdSpecification(paymentIntent.Id);
                var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);

                if (payment != null)
                {
                    payment.Cancel();
                    await _paymentRepository.UpdateAsync(payment);

                    _logger.LogInformation("Payment cancellation processed for: {PaymentId}", payment.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment canceled webhook");
            }
        }

        private async Task HandlePaymentRequiresAction(Event stripeEvent)
        {
            try
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent == null) return;

                _logger.LogInformation("Payment requires action: {PaymentIntentId}", paymentIntent.Id);

                // Log for monitoring - the frontend should handle the required action
                var paymentSpec = new PaymentByExternalIdSpecification(paymentIntent.Id);
                var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);

                if (payment?.Booking?.User != null)
                {
                    // Optionally send email about additional authentication required
                    _logger.LogInformation("Payment requires additional authentication: {PaymentId}", payment.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment requires action webhook");
            }
        }

        private async Task HandleDisputeCreated(Event stripeEvent)
        {
            try
            {
                var dispute = stripeEvent.Data.Object as Dispute;
                if (dispute == null) return;

                _logger.LogWarning("Dispute created for charge: {ChargeId}, Reason: {Reason}",
                    dispute.ChargeId, dispute.Reason);

                // Handle dispute logic here - notify admins, update payment status, etc.
                // For now, just log it for admin attention
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling dispute created webhook");
            }
        }

        private async Task HandleInvoicePaymentSucceeded(Event stripeEvent)
        {
            try
            {
                var invoice = stripeEvent.Data.Object as Invoice;
                if (invoice == null) return;

                _logger.LogInformation("Invoice payment succeeded: {InvoiceId}", invoice.Id);

                // Handle invoice payment success - useful for subscriptions
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling invoice payment succeeded webhook");
            }
        }

        private async Task HandleSubscriptionEvent(Event stripeEvent)
        {
            try
            {
                var subscription = stripeEvent.Data.Object as Subscription;
                if (subscription == null) return;

                _logger.LogInformation("Subscription event: {EventType} for {SubscriptionId}",
                    stripeEvent.Type, subscription.Id);

                // Handle subscription events - useful for recurring event registrations
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling subscription webhook");
            }
        }

        private async Task SendPaymentConfirmationEmail(Payment payment)
        {
            try
            {
                var subject = $"Payment Confirmation - {payment.Booking.Event.Title}";
                var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #28a745;'>Payment Confirmation</h2>
                        <p>Dear {payment.Booking.User.FullName},</p>
                        <p>Your payment has been successfully processed!</p>
                        
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 4px; margin: 20px 0;'>
                            <h3>Payment Details:</h3>
                            <p><strong>Payment Number:</strong> {payment.PaymentNumber}</p>
                            <p><strong>Event:</strong> {payment.Booking.Event.Title}</p>
                            <p><strong>Amount:</strong> {payment.Amount.Amount:C} {payment.Amount.Currency}</p>
                            <p><strong>Booking Number:</strong> {payment.Booking.BookingNumber}</p>
                            <p><strong>Payment Date:</strong> {payment.CompletedAt:yyyy-MM-dd HH:mm}</p>
                        </div>
                        
                        <p>Your booking is now confirmed. You will receive your tickets shortly.</p>
                        
                        <p>Best regards,<br>The Event Management Team</p>
                    </div>
                </body>
                </html>";

                await _emailService.SendEmailAsync(payment.Booking.User.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payment confirmation email for payment: {PaymentId}", payment.Id);
            }
        }

        private async Task SendPaymentFailureEmail(Payment payment, string failureReason)
        {
            try
            {
                var subject = $"Payment Failed - {payment.Booking.Event.Title}";
                var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #dc3545;'>Payment Failed</h2>
                        <p>Dear {payment.Booking.User.FullName},</p>
                        <p>Unfortunately, your payment could not be processed.</p>
                        
                        <div style='background-color: #f8d7da; padding: 15px; border-radius: 4px; margin: 20px 0; border-left: 4px solid #dc3545;'>
                            <h3>Payment Details:</h3>
                            <p><strong>Payment Number:</strong> {payment.PaymentNumber}</p>
                            <p><strong>Event:</strong> {payment.Booking.Event.Title}</p>
                            <p><strong>Amount:</strong> {payment.Amount.Amount:C} {payment.Amount.Currency}</p>
                            <p><strong>Reason:</strong> {failureReason}</p>
                        </div>
                        
                        <p>Please try again with a different payment method or contact your bank if the issue persists.</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='#' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Try Again</a>
                        </div>
                        
                        <p>If you continue to experience issues, please contact our support team.</p>
                        
                        <p>Best regards,<br>The Event Management Team</p>
                    </div>
                </body>
                </html>";

                await _emailService.SendEmailAsync(payment.Booking.User.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payment failure email for payment: {PaymentId}", payment.Id);
            }
        }
    }
}
