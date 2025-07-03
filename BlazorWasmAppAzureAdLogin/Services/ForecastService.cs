using BlazorWasmAppAzureAdLogin.Models;
using System.Net.Http.Json;

namespace BlazorWasmAppAzureAdLogin.Services
{
    public class WeatherForecastService
    {
        private readonly HttpClient _http;
        private readonly IHttpClientFactory clientFactory;
        public WeatherForecastService(HttpClient http, IHttpClientFactory ClientFactory)
        {
            clientFactory = ClientFactory;
            _http = clientFactory.CreateClient("MyApiClient");

        }

        public async Task<WeatherForecast[]?> GetForecastsAsync()
        {
            return await _http.GetFromJsonAsync<WeatherForecast[]?>("WeatherForecast");
        }
    }

}
