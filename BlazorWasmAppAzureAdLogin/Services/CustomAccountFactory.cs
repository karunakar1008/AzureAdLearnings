using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Security.Claims;

namespace BlazorWasmAppAzureAdLogin.Services
{
    public class CustomAccountFactory : AccountClaimsPrincipalFactory<CustomUserAccount>
    {
        private readonly ILogger<CustomAccountFactory> logger;
        private readonly IServiceProvider serviceProvider;

        public CustomAccountFactory(IAccessTokenProviderAccessor accessor,
            IServiceProvider serviceProvider,
            ILogger<CustomAccountFactory> logger)
            : base(accessor)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
            CustomUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var initialUser = await base.CreateUserAsync(account, options);

            if (initialUser.Identity is not null &&
                initialUser.Identity.IsAuthenticated)
            {
                var userIdentity = initialUser.Identity as ClaimsIdentity;

                if (userIdentity is not null)
                {
                    account?.Roles?.ForEach((role) =>
                    {
                        userIdentity.AddClaim(new Claim("appRole", role));
                    });

                    //account?.Wids?.ForEach((wid) =>
                    //{
                    //    userIdentity.AddClaim(new Claim("directoryRole", wid));
                    //});
                }
            }

            return initialUser;
        }
    }
}
