using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Common.Models.PaymentModels;
using Eventify.Application.Features.Payments.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Specifications.PaymentSpecifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eventify.Application.Features.Payments.Commands.ConfirmPayment
{
    public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result<ConfirmPaymentResult>>
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<ConfirmPaymentCommandHandler> _logger;

        public ConfirmPaymentCommandHandler(
            IRepository<Payment> paymentRepository,
            IPaymentService paymentService,
            ILogger<ConfirmPaymentCommandHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<Result<ConfirmPaymentResult>> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find payment by external transaction ID
                var paymentSpec = new PaymentByExternalIdSpecification(request.PaymentIntentId);
                var payment = await _paymentRepository.GetBySpecAsync(paymentSpec, cancellationToken);

                if (payment == null)
                {
                    return Result<ConfirmPaymentResult>.Failure("Payment not found");
                }

                // Process payment with Stripe
                var processRequest = new ProcessPaymentRequest
                {
                    PaymentIntentId = request.PaymentIntentId,
                    PaymentMethodId = request.PaymentMethodId ?? string.Empty
                };

                var paymentResult = await _paymentService.ProcessPaymentAsync(processRequest);

                if (paymentResult.IsSuccess)
                {
                    var stripeResult = paymentResult.Value;

                    if (stripeResult.Status == "succeeded")
                    {
                        payment.Complete(stripeResult.PaymentIntentId);
                        payment.UpdateProcessorResponse(System.Text.Json.JsonSerializer.Serialize(stripeResult));

                        // Update booking status to confirmed if payment successful
                        if (payment.Booking != null)
                        {
                            payment.Booking.Confirm();
                        }
                    }
                    else if (stripeResult.Status == "requires_action")
                    {
                        // Payment requires additional authentication
                        _logger.LogInformation("Payment requires action: {PaymentIntentId}", request.PaymentIntentId);
                    }
                    else
                    {
                        payment.Fail(stripeResult.FailureMessage ?? "Payment failed");
                    }

                    await _paymentRepository.UpdateAsync(payment, cancellationToken);

                    var result = new ConfirmPaymentResult
                    {
                        PaymentId = payment.Id,
                        Status = stripeResult.Status,
                        IsSuccessful = stripeResult.Status == "succeeded",
                        ReceiptUrl = stripeResult.ReceiptUrl,
                        FailureMessage = stripeResult.FailureMessage
                    };

                    _logger.LogInformation("Payment confirmed: {PaymentId} with status: {Status}",
                        payment.Id, stripeResult.Status);

                    return Result<ConfirmPaymentResult>.Success(result);
                }
                else
                {
                    payment.Fail(paymentResult.Error);
                    await _paymentRepository.UpdateAsync(payment, cancellationToken);

                    return Result<ConfirmPaymentResult>.Failure($"Payment confirmation failed: {paymentResult.Error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment: {PaymentIntentId}", request.PaymentIntentId);
                return Result<ConfirmPaymentResult>.Failure("An error occurred while confirming the payment");
            }
        }
    }
}
