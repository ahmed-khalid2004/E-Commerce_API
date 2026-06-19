using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Http;
using Shared.ErrorModels;

namespace E_Commerce.Web.CustomMiddleWares
{
    public class CustomExceptionHandlerMiddleWare(
        RequestDelegate next,
        ILogger<CustomExceptionHandlerMiddleWare> logger)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next.Invoke(httpContext);
                await HandleNotFoundEndPointAsync(httpContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something Went Wrong");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            var response = new ErrorToReturn { ErrorMessage = ex.Message };

            response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                OutOfStockException => StatusCodes.Status400BadRequest,
                BadRequestException b => GetBadRequestErrors(b, response),
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = response.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(response);
        }

        private static int GetBadRequestErrors(BadRequestException ex, ErrorToReturn response)
        {
            response.Errors = ex.Errors;
            return StatusCodes.Status400BadRequest;
        }

        private static async Task HandleNotFoundEndPointAsync(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var response = new ErrorToReturn
                {
                    ErrorMessage = $"End Point {httpContext.Request.Path} Is Not Found",
                    StatusCode = StatusCodes.Status404NotFound
                };
                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }
    }
}