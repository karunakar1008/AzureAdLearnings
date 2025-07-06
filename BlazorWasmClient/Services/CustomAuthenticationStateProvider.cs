namespace BlazorWasmClient.Services
{
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAccessTokenProvider _tokenProvider;

        public CustomAuthenticationStateProvider(IAccessTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenResult = await _tokenProvider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                //var claims = ParseClaimsFromJwt(token.Value);
                var decodedToken = DecodeJwt(token.Value);
                var claims = decodedToken.Claims;
                var identity = new ClaimsIdentity(claims, "Bearer");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        public static JwtSecurityToken DecodeJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }
    }

}
