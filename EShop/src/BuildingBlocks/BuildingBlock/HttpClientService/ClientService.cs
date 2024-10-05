using BuildingBlock.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BuildingBlock.HttpClientService
{
    public class ClientService : IClientService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpContext _httpContext;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ClientService> _logger;
        private readonly string requestId;
        public ClientService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor
            , IConfiguration configuration, ILogger<ClientService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContext = httpContextAccessor.HttpContext;
            _httpClient = _httpClientFactory.CreateClient();
            _logger = logger;

            //retrive request id from httpContext
            requestId = _httpContext.TraceIdentifier ?? "No Trace Identifier";

            //config httpClient
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };



        }

        //call api which is get method(without parameter)
        public async Task<T?> GetAll<T>(string relativeUrl, string? authenticationType = null, string? apiKey = null)
        {


            try
            {

                //append token to header
                if (!string.IsNullOrEmpty(apiKey))
                {
                    _httpClient.DefaultRequestHeaders.Remove(authenticationType); // Ensure no old key is lingering
                    _httpClient.DefaultRequestHeaders.Add(authenticationType, apiKey);
                }


                //call api
                _logger.LogInformation("TraceId:{id}. Calling external api with {Url} ", requestId, relativeUrl);
                var res = await _httpClient.GetAsync(relativeUrl);

                //check if api key is invalid
                if ((int)res.StatusCode == 401)
                {
                    _logger.LogError("TraceId:{id}. Calling external api with Url:{Url} is not authenticated. Exception:{Exception} ", requestId, relativeUrl, res.ReasonPhrase);
                    throw new ExternalApiException(res.ReasonPhrase?.ToString() ?? "Authentication failed");
                }

                //deserialize response data
                _logger.LogInformation("TraceId:{id}. Deserialize data from api ", requestId);
                var content = await res.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError("TraceId:{id}. Calling external api with Url:{Url} catch Exception:{Exception} ", requestId, relativeUrl, ex.Message);
                throw new ExternalApiException(ex.Message);
            }
        }

        //call api which is get method(with parameter)
        public async Task<T?> GetDetail<T>(string relativeUrl, string? param, string? authenticationType = null, string? apiKey = null)
        {
            try
            {

                //append token to header
                if (!string.IsNullOrEmpty(apiKey))
                {
                    _httpClient.DefaultRequestHeaders.Remove(authenticationType); // Ensure no old key is lingering
                    _httpClient.DefaultRequestHeaders.Add(authenticationType, apiKey);
                }


                //call api
                _logger.LogInformation("TraceId:{id}. Calling external api with {Url} ", requestId, relativeUrl + param);
                var res = await _httpClient.GetAsync(relativeUrl + param);

                //check if api key is invalid
                if ((int)res.StatusCode == 401)
                {
                    _logger.LogError("TraceId:{id}. Calling external api with Url:{Url} is not authenticated. Exception:{Exception} ", requestId, relativeUrl, res.ReasonPhrase);
                    throw new ExternalApiException(res.ReasonPhrase?.ToString() ?? "Authentication failed");
                }

                //deserialize response data
                _logger.LogInformation("TraceId:{id}. Deserialize data from api ", requestId);
                var content = await res.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                var deserializeObject = JsonSerializer.Deserialize<T>(content, _jsonSerializerOptions);
                return deserializeObject;
            }
            catch (Exception ex)
            {
                _logger.LogError("TraceId:{id}. Calling external api with Url:{Url} catch Exception:{Exception} ", requestId, relativeUrl, ex.Message);
                throw new ExternalApiException(ex.Message);
            }
        }



        public async Task<T?> PostSearch<T>(string relativeUrl, object? data, string? authenticationType = null, string? apiKey = null)
        {
            try
            {
                //append token to header
                if (!string.IsNullOrEmpty(apiKey))
                {
                    _httpClient.DefaultRequestHeaders.Remove(authenticationType); // Ensure no old key is lingering
                    _httpClient.DefaultRequestHeaders.Add(authenticationType, apiKey);
                }

                //call api
                _logger.LogInformation("TraceId:{id}. Calling external api with {Data} ", requestId, data);
                var res = await _httpClient.PostAsync(relativeUrl, GetBody(data));

                //check if api key is invalid
                if ((int)res.StatusCode == 401)
                {
                    await _httpContext.SignOutAsync("CookieAuthentication");
                    throw new ExternalApiException(res.ReasonPhrase?.ToString() ?? "Authentication failed");
                }

                //deserialize response data
                _logger.LogInformation("TraceId:{id}. Deserialize data from api ", requestId);
                var content = await res.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError("TraceId:{id}. Calling external api with Url:{Url} catch Exception:{Exception} ", requestId, relativeUrl, ex.Message);
                throw new ExternalApiException(ex.Message);
            }
        }


        //Serialize request body
        private StringContent? GetBody(object? data)
        {
            try
            {
                if (data == null) return null;
                var body = JsonSerializer.Serialize(data);
                return new StringContent(body, Encoding.UTF8, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError("TraceId:{id}. Serialize data catch Exception:{Exception} ", requestId, ex.Message);
                throw new ExternalApiException(ex.Message);
            }
        }
    }
}
