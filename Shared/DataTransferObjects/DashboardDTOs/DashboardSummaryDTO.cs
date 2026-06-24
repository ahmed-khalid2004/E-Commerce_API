namespace Shared.DataTransferObjects.DashboardDTOs
{
    public class DashboardSummaryDTO
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}