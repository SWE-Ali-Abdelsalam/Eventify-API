namespace Eventify.Application.Features.Admin.DTOs
{
    public class RevenueStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal RevenueThisYear { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalTransactions { get; set; }
        public int SuccessfulPayments { get; set; }
        public int FailedPayments { get; set; }
        public decimal RefundedAmount { get; set; }
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
    }
}
