using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ServiceAbstracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Attributes
{
     class CacheAttribute(int DurationInSec = 90) : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string CacheKey = CreateCacheKey(context.HttpContext.Request);

            ICacheService cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var CacheValue = await cacheService.GetAsync(CacheKey);

            if (CacheValue is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = CacheValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }

            var ExcutedContext = await next.Invoke();
            if (ExcutedContext.Result is OkObjectResult result)
            {
                await cacheService.SetAsync(CacheKey, result.Value, TimeSpan.FromSeconds(DurationInSec));
            }
        }

        private string CreateCacheKey(HttpRequest request)
        {
            StringBuilder Key = new StringBuilder();
            Key.Append(request.Path + '?');
            foreach (var Item in request.Query.OrderBy(Q => Q.Key))
            {
                Key.Append($"{Item.Key}={Item.Value}&");
            }
            return Key.ToString();
        }
    }
}
