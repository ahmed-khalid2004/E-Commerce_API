using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.DashboardDTOs;

namespace Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDTO>> GetSummary()
            => Ok(await serviceManager.DashboardService.GetSummaryAsync());

        [HttpGet("recent-orders")]
        public async Task<ActionResult<IReadOnlyList<RecentOrderDTO>>> GetRecentOrders([FromQuery] int limit = 10)
            => Ok(await serviceManager.DashboardService.GetRecentOrdersAsync(limit));

        [HttpGet("top-products")]
        public async Task<ActionResult<IReadOnlyList<TopProductDTO>>> GetTopProducts([FromQuery] int limit = 5)
            => Ok(await serviceManager.DashboardService.GetTopProductsAsync(limit));
    }
}