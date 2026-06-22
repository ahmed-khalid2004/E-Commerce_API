using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.OrderDTOs;

namespace Presentation.Controllers
{
    [Authorize]
    public class OrdersController(IServiceManager _serviceManager) : ApiBaseController
    {
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDTO>> CreateOrder(OrderDTO orderDTO)
        {
            var order = await _serviceManager.OrderService
                .CreateOrder(orderDTO, GetEmailFromToken(), GetUserIdFromToken());
            return Ok(order);
        }

        [AllowAnonymous]
        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethods()
            => Ok(await _serviceManager.OrderService.GetDeliveryMethodsAsync());

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderToReturnDTO>>> GetAllOrders()
            => Ok(await _serviceManager.OrderService.GetAllOrdersAsync(GetEmailFromToken()));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderToReturnDTO>> GetOrderById(Guid id)
            => Ok(await _serviceManager.OrderService.GetOrderByIdAsync(id, GetEmailFromToken()));

        // ── Admin ─────────────────────────────────────────────────────────────

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDTO>>> GetAllOrdersForAdmin()
            => Ok(await _serviceManager.OrderService.GetAllOrdersForAdminAsync());

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<OrderToReturnDTO>> UpdateOrderStatus(
            Guid id, [FromBody] UpdateOrderStatusDTO dto)
            => Ok(await _serviceManager.OrderService.UpdateOrderStatusAsync(id, dto.Status));
    }
}