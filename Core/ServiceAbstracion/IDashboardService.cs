using Shared.DataTransferObjects.DashboardDTOs;

namespace ServiceAbstracion
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDTO> GetSummaryAsync();
    }
}
