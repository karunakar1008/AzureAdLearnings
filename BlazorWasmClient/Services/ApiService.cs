using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorWasmClient.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpClientFactory clientFactory;
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly NavigationManager _navigation;

        public ApiService(HttpClient http, IHttpClientFactory ClientFactory, IAccessTokenProvider tokenProvider, NavigationManager navigation)
        {
            _http = http;
            clientFactory = ClientFactory;
            _tokenProvider = tokenProvider;
            _navigation = navigation;
            _http = clientFactory.CreateClient("BlazorWasmClientApi");
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await SendRequestWithToken(() => _http.GetAsync(url));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            return default;
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest content)
        {
            var response = await SendRequestWithToken(() => _http.PostAsJsonAsync(url, content));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            return default;
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string url, T content)
        {
            return await SendRequestWithToken(() => _http.PutAsJsonAsync(url, content));
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await SendRequestWithToken(() => _http.DeleteAsync(url));
        }

        // 🔁 Handles token injection, refresh, and 401/403 handling
        private async Task<HttpResponseMessage> SendRequestWithToken(Func<Task<HttpResponseMessage>> sendRequest)
        {
            var tokenResult = await _tokenProvider.RequestAccessToken(new AccessTokenRequestOptions
            {
                Scopes = new[] { "api://55fc27de-e189-46d9-847c-cc078e044881/access_as_user" },  // <-- Replace if needed
            });

            if (!tokenResult.TryGetToken(out var token))
            {
                _navigation.NavigateTo("authentication/login");
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Value);

            var response = await sendRequest();

            // Retry once on 401 with ForceRefresh=true
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                tokenResult = await _tokenProvider.RequestAccessToken(new AccessTokenRequestOptions
                {
                    Scopes = new[] { "api://55fc27de-e189-46d9-847c-cc078e044881/access_as_user" },
                });

                if (tokenResult.TryGetToken(out token))
                {
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token.Value);
                    response = await sendRequest();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _navigation.NavigateTo("authentication/login");
                }
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigation.NavigateTo("/unauthorized");
            }

            return response;
        }
    }

}
