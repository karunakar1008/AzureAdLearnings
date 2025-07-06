namespace BlazorWasmAppAzureAdLogin.Services
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string url);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest content);
        Task<HttpResponseMessage> PutAsync<T>(string url, T content);
        Task<HttpResponseMessage> DeleteAsync(string url);
    }
}
