using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.BasketModuleDTOs;

namespace Presentation.Controllers
{
    // All basket endpoints require login — "Add to Cart" is the action that forces auth.
    [Authorize]
    public class BasketsController(IServiceManager _serviceManager) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<BasketDTO>> GetBasket()
        {
            var basket = await _serviceManager.BasketService.GetBasketASync(GetUserIdFromToken());
            return Ok(basket);
        }

        [HttpPost]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdate(BasketDTO basket)
        {
            var result = await _serviceManager.BasketService
                .CreateOrUpdateBasketAsync(basket, GetUserIdFromToken());
            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket()
        {
            var result = await _serviceManager.BasketService.DeleteBasketASync(GetUserIdFromToken());
            return Ok(result);
        }
    }
}