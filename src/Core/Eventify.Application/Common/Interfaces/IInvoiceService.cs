using Eventify.Application.Common.Models.InvoiceModels;

namespace Eventify.Application.Common.Interfaces
{
    public interface IInvoiceService
    {
        Task<Result<InvoiceDocument>> GenerateInvoiceAsync(Guid bookingId);
        Task<Result<InvoiceDocument>> GenerateReceiptAsync(Guid paymentId);
        Task<Result<InvoiceDocument>> SendInvoiceEmailAsync(Guid bookingId, string email);
        Task<Result<byte[]>> GetInvoicePdfAsync(Guid invoiceId);
        Task<Result<InvoiceDocument>> UpdateInvoiceStatusAsync(Guid invoiceId, InvoiceStatus status);
    }
}
