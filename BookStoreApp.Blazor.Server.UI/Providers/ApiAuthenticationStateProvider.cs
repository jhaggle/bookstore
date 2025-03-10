using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookStoreApp.Blazor.Server.UI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorage;
        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

        // ADDED: We track if we've done our real "interactive" check or not.
        private bool _didSecondPass = false;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
            jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //So for the app at hand, I need to build what we call a claims principle.
            //And this claims principle is a collection of all the claims
            //and the claims are coming from the token
            //and the token came from our auth respons and now it's being stored in local storage.
            //I'm just showing you the hierarchy, right?
            // So the first thing that we need to do is retrieve that token from the local storage.

            var user = new ClaimsPrincipal(new ClaimsIdentity());

            // ADDED: If we haven't done the second pass, skip local storage calls 
            // to avoid "JS interop calls cannot be issued at this time"
            if (!_didSecondPass)
            {
                return new AuthenticationState(user);
            }
            if (_didSecondPass)
            {
                Console.WriteLine("hello");
            }

            var savedToken = await localStorage.GetItemAsync<string>("accessToken");
            if (savedToken == null)
            {
                // expects a claims principal, and if there is no token, there should be no claims principal
                // Remember claims principle depicts that there is a user present, right?
                // men sen ändrar han ju detta till att returnera user??
                return new AuthenticationState(user);
            }

            //The JWT is just one big string.
            //We've seen how it breaks up into the different parts.
            //So you could manually try to break it down and then try and pass the claims.
            //But we have built in libraries to help us with that.
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(savedToken);

            if (tokenContent.ValidTo < DateTime.Now)
            {
                await localStorage.RemoveItemAsync("accessToken");
                return new AuthenticationState(user);
            }

            var claims = await GetClaims();

            user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            return new AuthenticationState(user);
        }

        public async Task LoggedIn()
        {
            var claims = await GetClaims();
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task LoggedOut()
        {
            await localStorage.RemoveItemAsync("accessToken");
            var nobody = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(nobody));
            NotifyAuthenticationStateChanged(authState);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var savedToken = await localStorage.GetItemAsync<string>("accessToken");
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }

        public void MarkSecondPass()
        {
            if (!_didSecondPass)
            {
                _didSecondPass = true;
                Console.WriteLine("*** ApiAuthStateProvider: Setting _didSecondPass = true ***");
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
        }
    }
}
