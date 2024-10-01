
namespace BuildingBlock.HttpClientService
{
    public interface IClientService
    {
        Task<T?> GetAll<T>(string relativeUrl, string? authenticationType = null, string? apiKey = null);
        Task<T?> GetDetail<T>(string relativeUrl, string? param, string? authenticationType = null, string? apiKey = null);
        Task<T?> PostSearch<T>(string relativeUrl, object? data, string? authenticationType = null, string? apiKey = null);
    }
}