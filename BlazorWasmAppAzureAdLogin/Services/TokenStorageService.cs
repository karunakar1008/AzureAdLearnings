using Microsoft.JSInterop;

namespace BlazorWasmAppAzureAdLogin.Services
{
    public class TokenStorageService2
    {
        private readonly IJSRuntime js;
        private readonly string clientId = "01ad2845-680f-4c1b-929e-53811c79f27d"; // from app registration

        public TokenStorageService2(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task<string?> GetIdTokenAsync()
        {
            var key = $"msal.{clientId}.idtoken";
            return await js.InvokeAsync<string>("localStorage.getItem", key);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            // Access tokens are stored under cache keys — not easy to extract directly
            // Use IAccessTokenProvider for access tokens instead
            return null;
        }
    }

    public class TokenStorageService
    {
        private readonly IJSRuntime js;

        public TokenStorageService(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task<List<string>> GetAllSessionStorageKeysAsync()
        {
            return await js.InvokeAsync<List<string>>("tokenHelper.getAllSessionStorageKeys");
        }

        public async Task<string?> GetIdTokenByKeyAsync(string key)
        {
            return await js.InvokeAsync<string>("tokenHelper.getIdToken", key);
        }
    }

}
