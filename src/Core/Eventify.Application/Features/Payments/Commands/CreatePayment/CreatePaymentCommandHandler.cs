using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Common.Models.PaymentModels;
using Eventify.Application.Features.Payments.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Specifications.BookingSpecifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eventify.Application.Features.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<CreatePaymentResult>>
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IPaymentService _paymentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CreatePaymentCommandHandler> _logger;

        public CreatePaymentCommandHandler(
            IRepository<Booking> bookingRepository,
            IRepository<Payment> paymentRepository,
            IPaymentService paymentService,
            ICurrentUserService currentUserService,
            ILogger<CreatePaymentCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result<CreatePaymentResult>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get booking with details
                var bookingSpec = new BookingWithDetailsSpecification(request.BookingId);
                var booking = await _bookingRepository.GetBySpecAsync(bookingSpec, cancellationToken);

                if (booking == null)
                {
                    return Result<CreatePaymentResult>.Failure("Booking not found");
                }

                // Verify user owns this booking or is admin
                if (booking.UserId.ToString() != _currentUserService.UserId &&
                    !_currentUserService.IsInRole("Admin"))
                {
                    return Result<CreatePaymentResult>.Failure("You don't have permission to pay for this booking");
                }

                // Check if booking is in a payable state
                if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
                {
                    return Result<CreatePaymentResult>.Failure("This booking cannot be paid");
                }

                // Check if payment already exists and is successful
                var existingPayment = booking.Payments.FirstOrDefault(p => p.Status == PaymentStatus.Completed);
                if (existingPayment != null)
                {
                    return Result<CreatePaymentResult>.Failure("This booking has already been paid");
                }

                // Create payment intent with Stripe
                var paymentIntentRequest = new CreatePaymentIntentRequest
                {
                    Amount = booking.FinalAmount,
                    Currency = booking.TotalAmount.Currency,
                    PaymentMethodId = request.PaymentMethodId,
                    Description = $"Payment for Event: {booking.Event.Title}",
                    ReceiptEmail = request.ReceiptEmail ?? booking.User.Email,
                    BillingDetails = request.BillingDetails,
                    Metadata = new Dictionary<string, string>
                    {
                        ["booking_id"] = booking.Id.ToString(),
                        ["event_id"] = booking.EventId.ToString(),
                        ["user_id"] = booking.UserId.ToString()
                    }
                };

                var paymentIntentResult = await _paymentService.CreatePaymentIntentAsync(paymentIntentRequest);
                if (!paymentIntentResult.IsSuccess)
                {
                    return Result<CreatePaymentResult>.Failure($"Payment processing failed: {paymentIntentResult.Error}");
                }

                // Create payment record in our database
                var payment = new Payment(
                    booking.Id,
                    booking.FinalAmount,
                    PaymentMethod.CreditCard,
                    booking.TotalAmount.Currency
                );

                payment.Process(paymentIntentResult.Value.PaymentIntentId);
                payment.UpdateMetadata(System.Text.Json.JsonSerializer.Serialize(paymentIntentRequest.Metadata));

                if (request.BillingDetails != null)
                {
                    payment.UpdateBillingDetails(System.Text.Json.JsonSerializer.Serialize(request.BillingDetails));
                }

                var createdPayment = await _paymentRepository.AddAsync(payment, cancellationToken);

                var result = new CreatePaymentResult
                {
                    PaymentId = createdPayment.Id,
                    PaymentIntentId = paymentIntentResult.Value.PaymentIntentId,
                    ClientSecret = paymentIntentResult.Value.ClientSecret,
                    Status = paymentIntentResult.Value.Status,
                    Amount = paymentIntentResult.Value.Amount,
                    RequiresAction = paymentIntentResult.Value.Status == "requires_action"
                };

                _logger.LogInformation("Payment intent created for booking: {BookingId}, Payment: {PaymentId}",
                    booking.Id, createdPayment.Id);

                return Result<CreatePaymentResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for booking: {BookingId}", request.BookingId);
                return Result<CreatePaymentResult>.Failure("An error occurred while processing the payment");
            }
        }
    }
}
