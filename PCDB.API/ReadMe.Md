Secure ASP.NET Core 8 Web API with Azure AD and Swagger

    Step 1:Register your API in Azure AD

Go to the Azure Portal > Azure Active Directory > App registrations > New registration.

Name your API (e.g., MyApi).

Set Supported account types (usually "Accounts in this organizational directory only").

Set Redirect URI (optional for API).

Click Register.

Go to Expose an API > Set the Application ID URI (e.g., api://{client-id}).


    Step 2: Add scopes (e.g., access_as_user) or Configure API Permissions

In your app registration (e.g., MyApi), go to Expose an API.

Click Add a scope to define scopes (permissions) for your API, e.g., access_as_user.

Add the scope, setting user consent to required.


    Step 3: Register a client app (Swagger UI) in Azure AD

Register a new app for Swagger (e.g., MyApiClient).

Add a Redirect URI of type SPA or Web, such as https://localhost:5001/swagger/oauth2-redirect.html.

Under API permissions, add delegated permission to call your API (select the scope from your API app registration).


    4. Configure ASP.NET Core Web API

Create Web API project and select Open API while creating
    https://localhost:5001/swagger

Install NuGet Packages
		
		dotnet add package Swashbuckle.AspNetCore
		dotnet add package Microsoft.Identity.Web
		dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer


Update appsettings.json	

"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "Domain": "yourtenant.onmicrosoft.com",
  "TenantId": "your-tenant-id-guid",
  "ClientId": "your-api-client-id-guid",
  "SwaggerClientAppId": "SwaggerUI-Client-Id-guid",
}

Update Program.cs or Startup.cs

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
 builder.Services.AddSwaggerGen(c =>
{
    // Configure Swagger to use OAuth2 implicit flow for Azure AD
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                    {
                        { $"api://{builder.Configuration["AzureAd:ClientId"]}/.default", "Access API as user" }
                    // Or { $"api://{builder.Configuration["AzureAd:ClientId"]}/access_as_user", "Access API as user" }

                    }
            }
        }
    });

c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { 
            $"api://{builder.Configuration["AzureAd:ClientId"]}/.default" }
            // Or { $"api://{builder.Configuration["AzureAd:ClientId"]}/access_as_user", "Access API as user" }

        }
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

app.UseSwagger();
app.UseSwaggerUI(c =>
                {
                    c.OAuthClientId(builder.Configuration["AzureAd:SwaggerClientAppId"]);
                    c.OAuthScopes($"api://{builder.Configuration["AzureAd:ClientId"]}/.default");
                    //Or  c.OAuthScopes($"api://{azureAdSection["ClientId"]}/access_as_user");
                    c.OAuthUsePkce(); 
                });
app.UseAuthentication();
app.UseAuthorization();


    5. Protect Your Controllers or Endpoints

[Authorize]
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    // Your actions here
}

    6. Test Your API

How to Test in Swagger UI

Run your API app and 
Navigate to https://localhost:5001/swagger

Click Authorize button on Swagger UI.

A popup will appear for Azure AD login.

Login with your Azure AD credentials.

Swagger will acquire a token and attach it as a Bearer token for your API requests.

Now you can call your secured endpoints right from Swagger!

    7.Summary

Register your API and client app in Azure AD.

Configure ASP.NET Core 8 with Microsoft Identity Web for JWT Bearer authentication.

Add OAuth2 configuration in Swagger.

Use [Authorize] attribute on your API.

Test secured APIs in Swagger UI with Azure AD login.