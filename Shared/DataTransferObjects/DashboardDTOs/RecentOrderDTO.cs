namespace Shared.DataTransferObjects.DashboardDTOs
{
    public class RecentOrderDTO
    {
        public Guid Id { get; set; }
        public string BuyerEmail { get; set; } = default!;
        public DateTimeOffset OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = default!;
    }
}