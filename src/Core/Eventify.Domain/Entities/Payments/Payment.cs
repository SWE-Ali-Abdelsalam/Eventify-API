using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Entities.Payments
{
    public enum PaymentStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        Cancelled = 5,
        Refunded = 6,
        PartiallyRefunded = 7
    }

    public enum PaymentMethod
    {
        CreditCard = 1,
        DebitCard = 2,
        PayPal = 3,
        BankTransfer = 4,
        Cash = 5,
        Other = 6
    }

    public class Payment : BaseEntity, IAuditableEntity
    {
        public Guid BookingId { get; private set; }
        public string PaymentNumber { get; private set; } = string.Empty;
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public PaymentMethod Method { get; private set; }
        public Money Amount { get; private set; } = null!;
        public Money? RefundedAmount { get; private set; }
        public string? Currency { get; private set; }
        public string? ExternalTransactionId { get; private set; }
        public string? ExternalPaymentId { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? RefundedAt { get; private set; }
        public string? FailureReason { get; private set; }
        public string? ProcessorResponse { get; private set; }
        public string? BillingDetails { get; private set; } // JSON string
        public string? PaymentMetadata { get; private set; } // JSON string
        public bool IsInstallment { get; private set; }
        public int? InstallmentNumber { get; private set; }
        public int? TotalInstallments { get; private set; }

        // Audit properties
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Navigation properties
        public Booking Booking { get; private set; } = null!;
        public ICollection<PaymentRefund> Refunds { get; private set; } = new List<PaymentRefund>();

        private Payment() { } // EF Core

        public Payment(Guid bookingId, Money amount, PaymentMethod method, string? currency = null)
        {
            BookingId = bookingId;
            Amount = amount;
            Method = method;
            Currency = currency ?? amount.Currency;
            PaymentNumber = GeneratePaymentNumber();
        }

        private string GeneratePaymentNumber()
        {
            return $"PAY{DateTime.UtcNow:yyyyMMdd}{Id.ToString("N")[..8].ToUpper()}";
        }

        public void Process(string? externalTransactionId, string? processorResponse = null)
        {
            Status = PaymentStatus.Processing;
            ExternalTransactionId = externalTransactionId;
            ProcessorResponse = processorResponse;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete(string? externalPaymentId = null)
        {
            if (Status == PaymentStatus.Processing || Status == PaymentStatus.Pending)
            {
                Status = PaymentStatus.Completed;
                ExternalPaymentId = externalPaymentId;
                CompletedAt = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Fail(string failureReason)
        {
            Status = PaymentStatus.Failed;
            FailureReason = failureReason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == PaymentStatus.Pending || Status == PaymentStatus.Processing)
            {
                Status = PaymentStatus.Cancelled;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Refund(Money refundAmount, string reason)
        {
            if (Status != PaymentStatus.Completed)
                throw new InvalidOperationException("Can only refund completed payments");

            var totalRefunded = RefundedAmount ?? new Money(0, Amount.Currency);
            var newTotalRefunded = totalRefunded.Add(refundAmount);

            if (newTotalRefunded.Amount > Amount.Amount)
                throw new InvalidOperationException("Refund amount cannot exceed payment amount");

            RefundedAmount = newTotalRefunded;
            RefundedAt = DateTime.UtcNow;

            if (newTotalRefunded.Amount == Amount.Amount)
            {
                Status = PaymentStatus.Refunded;
            }
            else
            {
                Status = PaymentStatus.PartiallyRefunded;
            }

            var refund = new PaymentRefund(Id, refundAmount, reason);
            Refunds.Add(refund);

            UpdatedAt = DateTime.UtcNow;
        }

        public void SetInstallmentInfo(int installmentNumber, int totalInstallments)
        {
            IsInstallment = true;
            InstallmentNumber = installmentNumber;
            TotalInstallments = totalInstallments;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateBillingDetails(string billingDetails)
        {
            BillingDetails = billingDetails;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMetadata(string metadata)
        {
            PaymentMetadata = metadata;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the processor response data
        /// </summary>
        /// <param name="processorResponse">Response data from the payment processor</param>
        public void UpdateProcessorResponse(string processorResponse)
        {
            if (string.IsNullOrWhiteSpace(processorResponse))
                throw new ArgumentException("Processor response cannot be null or empty", nameof(processorResponse));

            ProcessorResponse = processorResponse;
            UpdatedAt = DateTime.UtcNow;
        }

        public Money RemainingAmount => Amount.Subtract(RefundedAmount ?? new Money(0, Amount.Currency));
        public bool IsFullyRefunded => RefundedAmount?.Amount == Amount.Amount;
        public bool CanBeRefunded => Status == PaymentStatus.Completed && !IsFullyRefunded;
        public bool IsSuccessful => Status == PaymentStatus.Completed;
        public bool IsFailed => Status == PaymentStatus.Failed;
        public bool IsPending => Status == PaymentStatus.Pending;
    }
}
