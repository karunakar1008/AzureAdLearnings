using Azure.Identity;
using BlazorAzureAdLogin.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph.Models.ExternalConnectors;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace BlazorAzureAdLogin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var initialScopes = builder.Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');

            builder.Configuration.AddAzureKeyVault(
            new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
            new VisualStudioCredential());

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                           .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
                               .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                                     .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi:Scopes"))
                                   .AddInMemoryTokenCaches();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();



            //builder.Services.AddAuthorization(options =>
            //{
            //    // By default, all incoming requests will be authorized according to the default policy
            //    //options.FallbackPolicy = options.DefaultPolicy;
            //});

            builder.Services.AddControllersWithViews()
             .AddMicrosoftIdentityUI();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddRazorPages();

            // Add consent handler
            builder.Services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();

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

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapRazorPages();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
