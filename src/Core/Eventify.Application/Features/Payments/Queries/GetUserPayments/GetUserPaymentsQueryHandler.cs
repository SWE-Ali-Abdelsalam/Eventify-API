using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Payments.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Specifications.PaymentSpecifications;
using MediatR;

namespace Eventify.Application.Features.Payments.Queries.GetUserPayments
{
    public class GetUserPaymentsQueryHandler : IRequestHandler<GetUserPaymentsQuery, Result<List<PaymentSummaryDto>>>
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetUserPaymentsQueryHandler(
            IRepository<Payment> paymentRepository,
            ICurrentUserService currentUserService)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<PaymentSummaryDto>>> Handle(GetUserPaymentsQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId ?? Guid.Parse(_currentUserService.UserId!);

            // Check if user has permission to view payments for the specified user
            if (request.UserId.HasValue && request.UserId.ToString() != _currentUserService.UserId &&
                !_currentUserService.IsInRole("Admin"))
            {
                return Result<List<PaymentSummaryDto>>.Failure("You don't have permission to view these payments");
            }

            var paymentsSpec = new PaymentsByUserSpecification(userId);
            paymentsSpec.ApplyPaging((request.PageNumber - 1) * request.PageSize, request.PageSize);

            var payments = await _paymentRepository.ListBySpecAsync(paymentsSpec, cancellationToken);

            var paymentDtos = payments.Select(p => new PaymentSummaryDto
            {
                Id = p.Id,
                PaymentNumber = p.PaymentNumber,
                Status = p.Status.ToString(),
                Amount = p.Amount.Amount,
                Currency = p.Amount.Currency,
                CreatedAt = p.CreatedAt,
                EventTitle = p.Booking.Event.Title,
                BookingNumber = p.Booking.BookingNumber
            }).ToList();

            return Result<List<PaymentSummaryDto>>.Success(paymentDtos);
        }
    }
}
