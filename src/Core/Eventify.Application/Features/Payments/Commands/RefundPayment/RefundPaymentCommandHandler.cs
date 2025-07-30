using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Common.Models.PaymentModels;
using Eventify.Application.Features.Payments.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Specifications.PaymentSpecifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eventify.Application.Features.Payments.Commands.RefundPayment
{
    public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, Result<RefundPaymentResult>>
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IPaymentService _paymentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<RefundPaymentCommandHandler> _logger;

        public RefundPaymentCommandHandler(
            IRepository<Payment> paymentRepository,
            IPaymentService paymentService,
            ICurrentUserService currentUserService,
            ILogger<RefundPaymentCommandHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result<RefundPaymentResult>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var paymentSpec = new PaymentWithDetailsSpecification(request.PaymentId);
                var payment = await _paymentRepository.GetBySpecAsync(paymentSpec, cancellationToken);

                if (payment == null)
                {
                    return Result<RefundPaymentResult>.Failure("Payment not found");
                }

                // Check if user has permission to refund (admin or booking owner)
                if (!_currentUserService.IsInRole("Admin") &&
                    payment.Booking.UserId.ToString() != _currentUserService.UserId)
                {
                    return Result<RefundPaymentResult>.Failure("You don't have permission to refund this payment");
                }

                // Check if payment can be refunded
                if (!payment.CanBeRefunded)
                {
                    return Result<RefundPaymentResult>.Failure("This payment cannot be refunded");
                }

                // Process refund with Stripe
                var refundRequest = new ProcessRefundRequest
                {
                    PaymentIntentId = payment.ExternalTransactionId!,
                    Amount = request.Amount,
                    Reason = request.Reason,
                    Metadata = new Dictionary<string, string>
                    {
                        ["payment_id"] = payment.Id.ToString(),
                        ["booking_id"] = payment.BookingId.ToString(),
                        ["requested_by"] = _currentUserService.UserId!,
                        ["notes"] = request.Notes ?? string.Empty
                    }
                };

                var refundResult = await _paymentService.ProcessRefundAsync(refundRequest);

                if (refundResult.IsSuccess)
                {
                    var stripeRefund = refundResult.Value;

                    // Update payment with refund information
                    payment.Refund(stripeRefund!.Amount, request.Reason);
                    await _paymentRepository.UpdateAsync(payment, cancellationToken);

                    var result = new RefundPaymentResult
                    {
                        PaymentId = payment.Id,
                        RefundId = stripeRefund.RefundId,
                        RefundAmount = stripeRefund.Amount,
                        Status = stripeRefund.Status,
                        ProcessedAt = stripeRefund.ProcessedAt
                    };

                    _logger.LogInformation("Payment refunded: {PaymentId}, Refund: {RefundId}, Amount: {Amount}",
                        payment.Id, stripeRefund.RefundId, stripeRefund.Amount.Amount);

                    return Result<RefundPaymentResult>.Success(result);
                }
                else
                {
                    return Result<RefundPaymentResult>.Failure($"Refund processing failed: {refundResult.Error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment: {PaymentId}", request.PaymentId);
                return Result<RefundPaymentResult>.Failure("An error occurred while processing the refund");
            }
        }
    }
}
