using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            return Ok(new
            {
                totalOrders = 0,
                totalRevenue = 0m,
                totalProducts = 0
            });
        }
    }
}
