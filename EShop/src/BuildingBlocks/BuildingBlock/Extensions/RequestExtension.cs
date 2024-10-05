using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Text;
using System.Text.Json;

namespace BuildingBlock.Extensions
{


    public class RequestLog
    {
        public string Type { get; set; }
        public string TraceIdentitfier { get; set; }
        public string Method { get; set; }
        public string UserId { get; set; }
        public DateTime RequestTime { get; set; }
        public string QueryString { get; set; }
        public string Path { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Parameters { get; set; }
        public string RequestBody { get; set; }
    }

    public class ResponseLog
    {
        public string Type { get; set; }
        public string TraceIdentifier { get; set; }
        public int StatusCode { get; set; }
        public DateTime ResponseTime { get; set; }
        public DateTime TimeHandled { get; set; }
        public string ResponseData { get; set; }

    }


    public static class RequestExtension
    {


        private static IHttpContextAccessor _httpContextAccessor;


        // Static constructor for initializing IHttpContextAccessor
        static RequestExtension()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }


        //format body of request
        public static async Task<string> FormatRequest(HttpRequest httpRequest)
        {



            //handle if request do not contain body
            if (httpRequest.Body is null)
            {
                return string.Empty;
            }


            httpRequest.EnableBuffering();  //allow request to be read multiple times

            //read the body
            var body = httpRequest.Body;
            var buffer = new byte[Convert.ToUInt32(httpRequest.ContentLength)];
            await httpRequest.Body.ReadAsync(buffer, 0, buffer.Length);

            //convert body content as string
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //resets the position of the request body stream to the beginning (position 0), so the body can be read again later by other parts of the application.
            httpRequest.Body.Position = 0;

            return bodyAsText;
        }

        //format body of response
        public static async Task<string> FormatResponse(HttpResponse httpResponse)
        {

            //ensure to read the response body from the beginning
            httpResponse.Body.Seek(0, SeekOrigin.Begin);

            //read the data and convert to string
            var text = await new StreamReader(httpResponse.Body).ReadToEndAsync();

            //reset the stream position
            httpResponse.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }


        //handle repsonse body if it is a list of object
        public static string FormatResponseToItemCountIfList(string responseData)
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

        //get the request information
        public static async Task<RequestLog> GetRequestDetail()
        {
            var httpRequest = _httpContextAccessor.HttpContext.Request; //retrive request
            var context = _httpContextAccessor.HttpContext;

            var routeData = context.GetRouteData();

            //create object of RequestLog
            var requestLog = new RequestLog()
            {
                TraceIdentitfier = context.TraceIdentifier,
                Action = routeData.Values?.ElementAt(0).Value.ToString() ?? string.Empty,
                Controller = routeData.Values?.ElementAt(1).Value.ToString() ?? string.Empty,
                Parameters = routeData is not null ? string.Join(", ", routeData.Values.Skip(2).Select(kv => $"{kv.Key}: {kv.Value}")) : string.Empty,
                Method = context.Request.Method,
                UserId = context.User.Identity?.Name ?? string.Empty,
                RequestTime = DateTime.UtcNow,
                QueryString = context.Request.QueryString.ToString() ?? string.Empty,
                Path = context.Request.Path,
                RequestBody = await FormatRequest(httpRequest) ?? string.Empty,
            };
            return requestLog;
        }


        //get the response information
        public static async Task<ResponseLog> GetResponseDetail()
        {
            var httpResponse = _httpContextAccessor.HttpContext.Response; //retrive request
            var context = _httpContextAccessor.HttpContext;

            //get route data from httpContext
            var routeData = context.GetRouteData();

            //format response body
            var responseBody = await FormatResponse(httpResponse) ?? string.Empty;
            var responseData = FormatResponseToItemCountIfList(responseBody) ?? string.Empty;


            //create object of RequestLog
            var responseLog = new ResponseLog()
            {
                TraceIdentifier = context.TraceIdentifier,
                StatusCode = httpResponse.StatusCode,
                ResponseTime = DateTime.UtcNow,
                ResponseData = responseData

            };
            return responseLog;
        }
    }
}
