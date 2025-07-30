using Eventify.Application.Common;
using Eventify.Application.Features.Payments.DTOs;

namespace Eventify.Application.Features.Payments.Queries.GetPayment
{
    public class GetPaymentQuery : BaseQuery<Result<PaymentDto>>
    {
        public Guid PaymentId { get; set; }
    }
}
