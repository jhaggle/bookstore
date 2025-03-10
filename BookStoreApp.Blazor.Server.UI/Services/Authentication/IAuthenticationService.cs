using BookStoreApp.Blazor.Server.UI.Services.Base;

namespace BookStoreApp.Blazor.Server.UI.Services.Authentication
{
    public interface IAuthenticationService
    {

        // This method when implemented should:
        // 1) call httpClient.LoginAsync(model)
        // 2) store token in local storage
        // 3) notify the AuthStateProvider about the new user
        //Task<Response<AuthResponse>> AuthenticateAsync(LoginUserDto loginModel);
        Task<bool> AuthenticateAsync(LoginUserDto loginModel);


        public Task Logout();
    }
}