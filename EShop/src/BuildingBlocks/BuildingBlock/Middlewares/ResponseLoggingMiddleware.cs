using BuildingBlock.Extensions;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Collections.Concurrent;

namespace BuildingBlock.Middlewares
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly ConcurrentDictionary<string, ILogger> _loggerCache = new();

        public ResponseLoggingMiddleware(RequestDelegate requestDelegate, string connectionString, string databaseName)
        {
            _requestDelegate = requestDelegate;
            _connectionString = connectionString;
            _databaseName = databaseName;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            var routeValues = context.Request.RouteValues;
            var collectionName = routeValues["controller"]?.ToString();

            //config logger
            var logger = _loggerCache.GetOrAdd(collectionName, name =>
           new LoggerConfiguration()
               .Enrich.With(new IgnorePropertiesEnricher("MessageTemplate", "RenderedMessage", "Timestamp", "UtcTimestamp"))
               .WriteTo.MongoDB(databaseUrl: $"{_connectionString}/{_databaseName}", collectionName: name)
               .CreateLogger());

            // Use a memory stream to capture the response body
            var originalBodyStream = context.Response.Body;


            using (var responseBodyStream = new MemoryStream())
            {
                context.Response.Body = responseBodyStream;

                try
                {
                    // Start the stopwatch to measure the request execution time
                    var stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();

                    await _requestDelegate(context); // Call the next middleware

                    // Stop the stopwatch when the response is ready to be sent
                    stopwatch.Stop();

                    // Log the time taken for the request execution
                    var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                    //get response detail
                    var responseLog = await RequestExtension.GetResponseDetail();
                    responseLog.Type = "Response";

                    //log response to database
                    logger.Information("Handle request. {Type}  {TraceId} {StatusCode} {ResponseTime} {ResponseData} {TimeHandled}",
                      responseLog.Type, responseLog.TraceIdentifier, responseLog.StatusCode, responseLog.ResponseTime, responseLog.ResponseData, elapsedMilliseconds);

                    // Copy the response back to the original body stream
                    await responseBodyStream.CopyToAsync(originalBodyStream);
                }
                finally
                {
                    // Restore the original response body stream
                    context.Response.Body = originalBodyStream;
                }
            }
        }
    }
}
