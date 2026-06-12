using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ServiceAbstracion;

namespace Presentation.Attributes
{
    /// <summary>
    /// Invalidates all Redis cache keys that start with the given prefix
    /// after a successful write operation (POST / PUT / DELETE).
    ///
    /// Usage:
    ///   [InvalidateCache("api/products")]          // clears all product cache keys
    ///   [InvalidateCache("api/products", "api/brands")] // clears multiple prefixes
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InvalidateCacheAttribute(params string[] prefixes) : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var executedContext = await next.Invoke();

            // Only invalidate on successful responses (2xx)
            if (executedContext.Result is ObjectResult { StatusCode: >= 200 and < 300 }
                or NoContentResult
                or CreatedAtActionResult)
            {
                var cacheService = context.HttpContext.RequestServices
                    .GetRequiredService<ICacheService>();

                foreach (var prefix in prefixes)
                    await cacheService.RemoveByPrefixAsync(prefix);
            }
        }
    }
}