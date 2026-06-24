using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.IdentityDTOs;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomersController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerDTO>>> GetAllCustomers()
            => Ok(await serviceManager.CustomerService.GetAllCustomersAsync());

        [HttpGet("{id}/stats")]
        public async Task<ActionResult<CustomerStatsDTO>> GetCustomerStats(string id)
            => Ok(await serviceManager.CustomerService.GetCustomerStatsAsync(id));

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            await serviceManager.CustomerService.DeleteCustomerAsync(id);
            return Ok();
        }
    }
}
