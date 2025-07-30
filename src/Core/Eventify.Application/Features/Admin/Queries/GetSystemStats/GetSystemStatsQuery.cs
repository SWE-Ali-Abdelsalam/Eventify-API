using Eventify.Application.Common;
using Eventify.Application.Features.Admin.DTOs;

namespace Eventify.Application.Features.Admin.Queries.GetSystemStats
{
    public class GetSystemStatsQuery : BaseQuery<Result<SystemStatsDto>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
