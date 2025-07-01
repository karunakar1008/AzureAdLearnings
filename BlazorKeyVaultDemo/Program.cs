using Azure.Identity;
using BlazorKeyVaultDemo.Components;
using BlazorKeyVaultDemo.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

namespace BlazorKeyVaultDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Load Key Vault URL from configuration or hardcode here
            string keyVaultUrl = $"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"; // <-- your key vault URL here

            // Create service to fetch secrets from Key Vault
            var keyVaultService = new KeyVaultService(keyVaultUrl);

            

            // Fetch client secret synchronously (acceptable during startup)
            string clientSecret = keyVaultService.GetSecretAsync("AzureADClientSecret").GetAwaiter().GetResult();


            //Different way of implementing
            var keyVaultService2 = new KeyVaultService2(builder.Configuration);
            string clientSecret2 = keyVaultService.GetSecretAsync("AzureAd--ClientSecret").GetAwaiter().GetResult();

            builder.Services.AddSingleton<KeyVaultService>();
            var initialScopes = builder.Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                               .AddMicrosoftIdentityWebApp(options =>
                               {
                                   // Bind AzureAd config from appsettings.json
                                   builder.Configuration.Bind("AzureAd", options);

                                   // Override client secret fetched securely from Key Vault
                                   options.ClientSecret = clientSecret;
                               })
                               .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                                     .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi:Scopes"))
                                   .AddInMemoryTokenCaches();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
