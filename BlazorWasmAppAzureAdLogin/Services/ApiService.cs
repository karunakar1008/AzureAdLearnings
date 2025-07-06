using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorWasmAppAzureAdLogin.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _http;
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly NavigationManager _navigation;

        public ApiService(HttpClient http, IAccessTokenProvider tokenProvider, NavigationManager navigation)
        {
            _http = http;
            _tokenProvider = tokenProvider;
            _navigation = navigation;
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

        //private async Task<HttpResponseMessage> SendRequestWithToken(Func<Task<HttpResponseMessage>> sendRequest)
        //{
        //    var tokenResult = await _tokenProvider.RequestAccessToken();

        //    if (!tokenResult.TryGetToken(out var token))
        //    {
        //        // Not logged in — navigate to login
        //        _navigation.NavigateTo("authentication/login");
        //        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //    }

        //    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

        //    var response = await sendRequest();

        //    if (response.StatusCode == HttpStatusCode.Unauthorized)
        //    {
        //        _navigation.NavigateTo("authentication/login");
        //    }
        //    else if (response.StatusCode == HttpStatusCode.Forbidden)
        //    {
        //        _navigation.NavigateTo("/unauthorized");
        //    }

        //    return response;
        //}

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


    public class ApiService2 : IApiService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigation;

        public ApiService2(HttpClient http, NavigationManager navigation)
        {
            _http = http;
            _navigation = navigation;
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                return await _http.GetFromJsonAsync<T>(url);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect(); // Redirects to login
                return default;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                _navigation.NavigateTo("/unauthorized");
                return default;
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest content)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TResponse>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _navigation.NavigateTo("/unauthorized");
                }
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }

            return default;
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string url, T content)
        {
            try
            {
                return await _http.PutAsJsonAsync(url, content);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            try
            {
                return await _http.DeleteAsync(url);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}
