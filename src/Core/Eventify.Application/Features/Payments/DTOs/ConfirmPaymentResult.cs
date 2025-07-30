namespace Eventify.Application.Features.Payments.DTOs
{
    public class ConfirmPaymentResult
    {
        public Guid PaymentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public string? ReceiptUrl { get; set; }
        public string? FailureMessage { get; set; }
    }
}
