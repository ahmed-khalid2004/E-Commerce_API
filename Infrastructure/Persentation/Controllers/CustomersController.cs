using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.IdentityDTOs;
using Shared.DataTransferObjects.OrderDTOs;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomersController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerDTO>>> GetAllCustomers()
            => Ok(await serviceManager.CustomerService.GetAllCustomersAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomerById(string id)
            => Ok(await serviceManager.CustomerService.GetCustomerByIdAsync(id));

        [HttpGet("{id}/orders")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDTO>>> GetCustomerOrders(string id)
            => Ok(await serviceManager.CustomerService.GetCustomerOrdersAsync(id));

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