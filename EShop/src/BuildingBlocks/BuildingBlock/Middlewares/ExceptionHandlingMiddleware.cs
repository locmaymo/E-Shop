using BuildingBlock.Exceptions.Handlers;
using Microsoft.AspNetCore.Http;

namespace BuildingBlock.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICustomExceptionHandler _exceptionHandler;

        public ExceptionHandlingMiddleware(RequestDelegate next, ICustomExceptionHandler exceptionHandler)
        {
            _next = next;
            _exceptionHandler = exceptionHandler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.TryHandleAsync(context, ex, CancellationToken.None);
            }
        }
    }
}
