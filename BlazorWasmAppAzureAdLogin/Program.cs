using BlazorWasmAppAzureAdLogin.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorWasmAppAzureAdLogin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<WeatherForecastService>();

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                //options.ProviderOptions.DefaultAccessTokenScopes.Add("api://55fc27de-e189-46d9-847c-cc078e044881/API.Access");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("api://55fc27de-e189-46d9-847c-cc078e044881/access_as_user");
                // Optional: Get roles from token
                options.UserOptions.RoleClaim = "role";
            });

            builder.Services.AddHttpClient("MyApiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7168/"); // your API URL
            })
            .AddHttpMessageHandler(sp =>
            {
                var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler(
                        authorizedUrls: new[] { "https://localhost:7168" },
                        scopes: new[] { "api://55fc27de-e189-46d9-847c-cc078e044881/access_as_user" }
                    );

                // Register a typed service you can inject
                builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MyApiClient"));

                return handler;
            });

            await builder.Build().RunAsync();
        }
    }
}
