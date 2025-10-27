using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared;
using Shared.DataTransferObjects.BasketModuleDTOs;
using Shared.DataTransferObjects.ProductModuleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
   // BaseUrl: http://localhost:5239/api/Basket
    public class BasketsController(IServiceManager _serviceManager) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<BasketDTO>> GetBasket(string Key)
        {
            var Basket = await _serviceManager.BasketService.GetBasketASync(Key);
            return Ok(Basket);
        }


        [HttpPost]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdate(BasketDTO basket)
        {
            var Basket = await _serviceManager.BasketService.CreateOrUpdateBasketAsync(basket);
            return Ok(Basket);
        }

        [HttpDelete("{Key}")]
        public async Task<ActionResult<bool>> DeleteBasket(string Key)
        {
            var Result = await _serviceManager.BasketService.DeleteBasketASync(Key);
            return Ok(Result);
        }

    }

}