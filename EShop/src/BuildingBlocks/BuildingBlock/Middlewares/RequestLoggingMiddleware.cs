using BuildingBlock.Extensions;
using Microsoft.AspNetCore.Http;

using Serilog;
using System.Collections.Concurrent;

namespace BuildingBlock.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly ConcurrentDictionary<string, ILogger> _loggerCache = new();

        public RequestLoggingMiddleware(RequestDelegate requestDelegate, string connectionString, string databaseName)
        {
            _requestDelegate = requestDelegate;
            _connectionString = connectionString;
            _databaseName = databaseName;

        }


        public async Task InvokeAsync(HttpContext context)
        {

            //get detail of request
            var requestLog = await RequestExtension.GetRequestDetail();
            requestLog.Type = "Request";

            //set up collection name to be same as controller name
            var collectionName = requestLog.Controller;

            //config logger
            var logger = LoggerExtension.GetLogger(_connectionString, _databaseName, collectionName);


            _ = Task.Run(() =>
            {
                try
                {
                    //log request to database
                    logger.Information("Receive request.{Type} {TraceId} {RequestTime} {UserId} {RequestPath} {Method} {Action} {Controller} {QueryString} {Parameters} {RequestBody} ",
                   requestLog.Type, requestLog.TraceIdentitfier, requestLog.RequestTime, requestLog.UserId, requestLog.Path, requestLog.Method
                   , requestLog.Action, requestLog.Controller, requestLog.QueryString, requestLog.Parameters, requestLog.RequestBody);
                }
                catch (Exception ex)
                {
                    // Handle logging errors (for example, log to console or fallback logging mechanism)
                    Console.WriteLine($"Failed to log request: {ex.Message}");
                }


            });

            //move to next middleware
            await _requestDelegate(context);

        }



    }
}
