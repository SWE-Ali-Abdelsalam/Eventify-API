using Eventify.Application.Common.Models.PaymentModels;

namespace Eventify.Application.Common.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request);
        Task<Result<PaymentResult>> ProcessPaymentAsync(ProcessPaymentRequest request);
        Task<Result<RefundResult>> ProcessRefundAsync(ProcessRefundRequest request);
        Task<Result<PaymentResult>> CapturePaymentAsync(string paymentIntentId);
        Task<Result<PaymentResult>> CancelPaymentAsync(string paymentIntentId);
        Task<Result<PaymentResult>> ValidateWebhookSignatureAsync(string payload, string signature);
        Task<Result<PaymentResult>> HandleWebhookEventAsync(string eventType, string eventData);
        Task<Result<CustomerResult>> CreateCustomerAsync(CreateCustomerRequest request);
        Task<Result<CustomerResult>> UpdateCustomerAsync(string customerId, UpdateCustomerRequest request);
        Task<Result<InvoiceResult>> CreateInvoiceAsync(CreateInvoiceRequest request);
        Task<Result<SubscriptionResult>> CreateSubscriptionAsync(CreateSubscriptionRequest request);
    }
}
