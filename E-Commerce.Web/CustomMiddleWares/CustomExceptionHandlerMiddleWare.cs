using DomainLayer.Exceptions;
using Shared.ErrorModels;

namespace E_Commerce.Web.CustomMiddleWares
{
    public class CustomExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleWare> _logger;

        public CustomExceptionHandlerMiddleWare(RequestDelegate Next,ILogger<CustomExceptionHandlerMiddleWare>logger)
        {
            _next = Next;
            _logger = logger;
        }

        public async Task InvokeASync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
                await HandleNotFoundEndPointASync(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something Went Wrong");

                await HandleExceptionASync(httpContext, ex);

            }


        }

        private static async Task HandleExceptionASync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var Response = new ErrorToReturn()
            {
                ErrorMessage = ex.Message,
                StatusCode = httpContext.Response.StatusCode,
            };

            await httpContext.Response.WriteAsJsonAsync(Response);
        }

        private static async Task HandleNotFoundEndPointASync(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var Response = new ErrorToReturn()
                {
                    ErrorMessage = $"End Point {httpContext.Request.Path} Is Not Found",
                    StatusCode = StatusCodes.Status404NotFound,
                };

                await httpContext.Response.WriteAsJsonAsync(Response);
            }
        }
    }
}
