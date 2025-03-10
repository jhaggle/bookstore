using System.Threading.Channels;
using System;
using Blazored.LocalStorage;
using BookStoreApp.Blazor.Server.UI.Providers;
using BookStoreApp.Blazor.Server.UI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BookStoreApp.Blazor.Server.UI.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IClient httpClient;
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        // TEMP STORAGE for SSR pass
        private string? _pendingToken;
        public bool HasPendingToken => !string.IsNullOrEmpty(_pendingToken);

        //So this is going to be a wrapper around the API call, which means that I need a copy of I client here.
        // So we inject IClient här eftersom vi har registrerat IClient i Program.cs
        //I also need a copy of local storage, because I need to store the token in local storage
        public AuthenticationService(
            IClient httpClient,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        // This method
        // 1) call httpClient.LoginAsync(model)
        // 2) store token in local storage
        // 3) notify the AuthStateProvider about the new user
        // 
        // In practice, for SSR, we do NOT call localStorage here. 
        // We stash the token in _pendingToken, then we commit it later once the client is interactive.
        public async Task<bool> AuthenticateAsync(LoginUserDto loginModel)
        {
            bool response = true;
            AuthResponse? result = null;

            try
            {
                // 1) call httpClient
                result = await httpClient.LoginAsync(loginModel);
                _pendingToken = result.Token;

                // (We do NOT directly call localStorage or AuthProvider here, 
                //  to avoid "JS interop calls cannot be issued at this time.")
            }
            //catch (ApiException exception)
            //{
            //    //response = ConvertApiExceptions<AuthResponse>(exception);
            //    response = false;
            //}
            catch (ApiException ex)
            {
                // If the server returns 202 or another 2xx not =200, we get an exception
                if (ex.StatusCode >= 200 && ex.StatusCode <= 299)
                {
                    // We'll parse the JSON from ex.Response to get the token
                    var fallbackResult = JsonConvert.DeserializeObject<AuthResponse>(ex.Response);
                    if (fallbackResult != null && !string.IsNullOrEmpty(fallbackResult.Token))
                    {
                        // Stash the token in memory only
                        _pendingToken = fallbackResult.Token;

                        // No localStorage calls here => defers until "CommitPendingTokenAsync"
                        return true;
                    }
                    // If we can't parse the token for some reason, just fail
                    response = false;
                    return response;
                }
            }

            return response;
        }

        // Called from 'MyAuthRenderer' OnAfterRender, or an on-click event after the client is connected
        public async Task CommitPendingTokenAsync()
        {
            if (!string.IsNullOrEmpty(_pendingToken))
            {
                // Now that we're in real interactive mode, we can set local storage
                await localStorage.SetItemAsync("accessToken", _pendingToken);

                // Clear the pending token after storing
                _pendingToken = null;

                // 3) Change auth state of app
                //When we need to change the authentication state of the app, 
                //that is when we call on our custom method.
                await ((ApiAuthenticationStateProvider)authenticationStateProvider).LoggedIn();
            }
        }

        public async Task Logout()
        {
            await ((ApiAuthenticationStateProvider)authenticationStateProvider).LoggedOut();
        }
    }
}
