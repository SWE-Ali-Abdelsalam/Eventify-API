using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Payments.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Specifications.PaymentSpecifications;
using MediatR;

namespace Eventify.Application.Features.Payments.Queries.GetPayment
{
    public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, Result<PaymentDto>>
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetPaymentQueryHandler(
            IRepository<Payment> paymentRepository,
            ICurrentUserService currentUserService)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PaymentDto>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
        {
            var paymentSpec = new PaymentWithDetailsSpecification(request.PaymentId);
            var payment = await _paymentRepository.GetBySpecAsync(paymentSpec, cancellationToken);

            if (payment == null)
            {
                return Result<PaymentDto>.Failure("Payment not found");
            }

            // Check if user has permission to view this payment
            if (!_currentUserService.IsInRole("Admin") &&
                payment.Booking.UserId.ToString() != _currentUserService.UserId)
            {
                return Result<PaymentDto>.Failure("You don't have permission to view this payment");
            }

            var paymentDto = new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                PaymentNumber = payment.PaymentNumber,
                Status = payment.Status.ToString(),
                Method = payment.Method.ToString(),
                Amount = payment.Amount.Amount,
                Currency = payment.Amount.Currency,
                RefundedAmount = payment.RefundedAmount?.Amount,
                ExternalTransactionId = payment.ExternalTransactionId,
                ProcessedAt = payment.ProcessedAt,
                CompletedAt = payment.CompletedAt,
                FailureReason = payment.FailureReason,
                Booking = new BookingSummaryDto
                {
                    Id = payment.Booking.Id,
                    BookingNumber = payment.Booking.BookingNumber,
                    EventTitle = payment.Booking.Event.Title,
                    UserName = payment.Booking.User.FullName,
                    TotalTickets = payment.Booking.TotalTickets
                },
                Refunds = payment.Refunds.Select(r => new PaymentRefundDto
                {
                    Id = r.Id,
                    Amount = r.Amount.Amount,
                    Currency = r.Amount.Currency,
                    Reason = r.Reason,
                    RefundedAt = r.RefundedAt
                }).ToList()
            };

            return Result<PaymentDto>.Success(paymentDto);
        }
    }
}
