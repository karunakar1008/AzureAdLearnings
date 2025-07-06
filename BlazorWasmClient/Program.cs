using BlazorWasmClient.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace BlazorWasmClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");


            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7283/") });


            builder.Services.AddHttpClient("BlazorWasmClientApi", client =>
            client.BaseAddress = new Uri("https://localhost:7283/"))
          .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorWasmClientApi"));


            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("api://55fc27de-e189-46d9-847c-cc078e044881/access_as_user");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("User.Read");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
                options.UserOptions.RoleClaim = ClaimTypes.Role;
                options.ProviderOptions.LoginMode = "redirect";
            });

            builder.Services.AddAuthorizationCore();
            
            builder.Services.AddScoped<AccountClaimsPrincipalFactory<RemoteUserAccount>, CustomUserFactory>();

            builder.Services.AddScoped<IApiService, ApiService>();

         
            await builder.Build().RunAsync();
        }
    }
}
