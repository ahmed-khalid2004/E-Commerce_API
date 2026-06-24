using Shared.DataTransferObjects.DashboardDTOs;
namespace ServiceAbstracion
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDTO> GetSummaryAsync();
        Task<IReadOnlyList<RecentOrderDTO>> GetRecentOrdersAsync(int limit);
        Task<IReadOnlyList<TopProductDTO>> GetTopProductsAsync(int limit);
    }
}