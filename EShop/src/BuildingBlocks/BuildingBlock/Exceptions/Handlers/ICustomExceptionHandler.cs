using Microsoft.AspNetCore.Http;

namespace BuildingBlock.Exceptions.Handlers
{
    public interface ICustomExceptionHandler
    {
        ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken);
    }
}
