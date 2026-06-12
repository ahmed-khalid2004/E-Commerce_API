using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.BasketModuleDTOs;

namespace Presentation.Controllers
{
    public class PaymentsController(IServiceManager _serviceManager) : ApiBaseController
    {
        // Authenticated — payment intent is tied to a real user session
        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var paymentIntent = await _serviceManager.PaymentService
                .CreateOrUpdatePaymentIntentAsync(basketId);
            return Ok(paymentIntent);
        }

    }
}