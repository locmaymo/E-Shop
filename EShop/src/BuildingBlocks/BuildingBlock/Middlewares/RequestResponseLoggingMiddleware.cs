using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Serilog;
using System.Text;
using System.Text.Json;

namespace BuildingBlock.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate requestDelegate, string connectionString, string databaseName, string collectionName)
        {
            _requestDelegate = requestDelegate;
            _logger = new LoggerConfiguration()
            .WriteTo.MongoDB(databaseUrl: $"{connectionString}/{databaseName}", collectionName: collectionName)
            .CreateLogger();
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var traceIdentifier = context.TraceIdentifier;
            var type = context.Request.Method;
            var requestTime = DateTime.UtcNow;
            var userId = context.User.Identity?.Name ?? string.Empty;
            var queryString = context.Request.QueryString.ToString() ?? string.Empty;
            var routeData = context.GetRouteData();
            var routeParams = routeData is not null ? string.Join(", ", routeData.Values.Select(kv => $"{kv.Key}: {kv.Value}")) : string.Empty;
            var requestBody = await FormatRequest(context.Request);

            var originalResponseBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                try
                {
                    await _requestDelegate(context);
                }
                finally
                {
                    var responseTime = DateTime.UtcNow;
                    var statusCode = context.Response.StatusCode;
                    string responseData = await FormatResponse(context.Response);
                    responseData = FormatResponseToItemCountIfList(responseData);


                    _logger.Information("Handle request. {TraceId} {StatusCode} {UserId} {RequestTime} {QueryString} {RouteParams} {RequestBody} {ResponseData}",
                   traceIdentifier, statusCode, userId, requestTime, queryString, routeParams, requestBody, responseData);

                    await responseBody.CopyToAsync(originalResponseBodyStream); // return the response to the client
                }
            }



        }


        private async Task<string> FormatRequest(HttpRequest request)
        {
            if (request.Body is null)
            {
                return string.Empty;
            }
            request.EnableBuffering();
            var body = request.Body;
            var buffer = new byte[Convert.ToUInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;
            return bodyAsText;
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }

        private string FormatResponseToItemCountIfList(string responseData)
        {
            try
            {
                // Try to deserialize the response to a list of objects
                var items = JsonSerializer.Deserialize<List<object>>(responseData);
                if (items != null)
                {
                    // Return the count of items as the response data
                    return items.Count.ToString() + " items";
                }
            }
            catch (JsonException)
            {
                return responseData;
            }
            return responseData;

        }
    }
}
