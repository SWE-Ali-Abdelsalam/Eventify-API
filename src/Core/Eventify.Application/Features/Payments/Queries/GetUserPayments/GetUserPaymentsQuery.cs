using Eventify.Application.Common;
using Eventify.Application.Features.Payments.DTOs;

namespace Eventify.Application.Features.Payments.Queries.GetUserPayments
{
    public class GetUserPaymentsQuery : BaseQuery<Result<List<PaymentSummaryDto>>>
    {
        public Guid? UserId { get; set; } // Optional - if null, use current user
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
