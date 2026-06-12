using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.BasketModuleDTOs;

namespace Presentation.Controllers
{
    public class BasketsController(IServiceManager _serviceManager) : ApiBaseController
    {
        // Public — guest shopping carts are allowed (basket key is the identifier)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<BasketDTO>> GetBasket(string Key)
        {
            var basket = await _serviceManager.BasketService.GetBasketASync(Key);
            return Ok(basket);
        }

        // Public — guest can build a basket before logging in
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdate(BasketDTO basket)
        {
            var result = await _serviceManager.BasketService.CreateOrUpdateBasketAsync(basket);
            return Ok(result);
        }

        // Authenticated — only the owner should delete their basket
        [Authorize]
        [HttpDelete("{Key}")]
        public async Task<ActionResult<bool>> DeleteBasket(string Key)
        {
            var result = await _serviceManager.BasketService.DeleteBasketASync(Key);
            return Ok(result);
        }
    }
}