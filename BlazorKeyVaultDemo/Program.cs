using Azure.Identity;
using BlazorKeyVaultDemo.Components;
using BlazorKeyVaultDemo.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

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

            var configuration = builder.Configuration;

            //Different way of implementing
            //var keyVaultService2 = new KeyVaultService2(builder.Configuration);
            //string clientSecret2 = keyVaultService.GetSecretAsync("AzureAd--ClientSecret").GetAwaiter().GetResult();

            //builder.Services.AddSingleton<KeyVaultService>();

            builder.Services.AddSingleton(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<KeyVaultService>>();
                return new KeyVaultService3(keyVaultUrl, logger);
            });

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


            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                //options.FallbackPolicy = options.DefaultPolicy;
            });

            builder.Services.AddControllersWithViews()
          .AddMicrosoftIdentityUI();

            //Learning: You can define here or Router component with <CascadingAuthenticationState>
            //builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddRazorPages();

            //// Add consent handler
            //builder.Services.AddServerSideBlazor()
            //    .AddMicrosoftIdentityConsentHandler();

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Key vault secret:{clientSecret}");

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


            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
