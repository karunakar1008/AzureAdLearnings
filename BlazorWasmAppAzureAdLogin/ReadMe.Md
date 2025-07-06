Step 1: Register apps in Azure AD
You need two app registrations in Azure AD:

Blazor WebAssembly client app — this is your front-end.

Web API app — your backend API secured by Azure AD.

Register the API app

In Azure Portal → Azure AD → App registrations → New registration.

Name: MySecureApi.

Supported account types: choose your scenario (e.g., single tenant).

Leave Redirect URI empty.

After creation:

Go to Expose an API → Set Application ID URI (e.g., api://<api-client-id>).

Add a scope, e.g., access_as_user.

Register the Blazor app
New registration → Name: MyBlazorClient.

Supported account types: same as API.

Redirect URI: add type SPA → https://localhost:5001/authentication/login-callback.

Under API Permissions → Add a permission → My APIs → select your API → select the scope you created (access_as_user).

Under Authentication, check Access tokens and ID tokens.

Step 2: Configure Blazor WebAssembly client

 "AzureAd": {
    "Authority": "https://login.microsoftonline.com/<tenant-id>",
    "ClientId": "<client-id-of-blazor-app>",
    "ValidateAuthority": true,
    "DefaultScopes": [ "api://<api-client-id>/access_as_user" ]
  }

DefaultScopes: This is the scope defined under Expose an API section in the API app registration
