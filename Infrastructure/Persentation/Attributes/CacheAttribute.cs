using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ServiceAbstracion;
using System.Text;

namespace Presentation.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CacheAttribute(int DurationInSec = 90) : ActionFilterAttribute
    {
        private const bool CacheEnabled = true;

          

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (!CacheEnabled)
            {
                await next();
                return;
            }
            var cacheService = context.HttpContext.RequestServices
                .GetRequiredService<ICacheService>();

            string cacheKey = CacheKeyBuilder.Build(context.HttpContext.Request);

            var cachedValue = await cacheService.GetAsync(cacheKey);


            if (cachedValue is not null)
            {
                context.Result = new ContentResult
                {
                    Content = cachedValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }

            var executedContext = await next.Invoke();

            if (executedContext.Result is OkObjectResult result)
            {
                await cacheService.SetAsync(
                    cacheKey,
                    result.Value!,
                    TimeSpan.FromSeconds(DurationInSec));
            }
        }
    }
}
