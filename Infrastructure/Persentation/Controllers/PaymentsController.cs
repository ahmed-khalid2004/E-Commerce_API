using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.BasketModuleDTOs;

namespace Presentation.Controllers
{
    [Authorize]
    public class PaymentsController(IServiceManager _serviceManager) : ApiBaseController
    {
        // basketId no longer comes from the URL — resolved from the JWT.
        // deliveryMethodId is sent explicitly in the request body.
        [HttpPost]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdatePaymentIntent(
            [FromBody] CreatePaymentIntentDTO dto)
        {
            var paymentIntent = await _serviceManager.PaymentService
                .CreateOrUpdatePaymentIntentAsync(GetUserIdFromToken(), dto.DeliveryMethodId);
            return Ok(paymentIntent);
        }
    }
}