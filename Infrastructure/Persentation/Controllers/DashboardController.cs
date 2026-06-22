using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.DashboardDTOs;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDTO>> GetSummary()
            => Ok(await serviceManager.DashboardService.GetSummaryAsync());
    }
}
