using Blazored.LocalStorage;
using BookStoreApp.Blazor.Server.UI.Components; // Detta är ju app!?
using BookStoreApp.Blazor.Server.UI.Providers;
using BookStoreApp.Blazor.Server.UI.Services.Authentication;
using BookStoreApp.Blazor.Server.UI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Endpoints;


namespace BookStoreApp.Blazor.Server.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents() //Blazor is now part of razorcomponents system. Registers Blazor components instead of separate Blazor Server/WebAssembly services.
                .AddInteractiveServerComponents();

            builder.Services.AddScoped<AuthenticationService>(); // Register the class itself

            builder.Services.AddBlazoredLocalStorage(); 

            //builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<IClient, Client>(client => client.BaseAddress = new Uri("https://localhost:7126")); //cl == client

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(); //To make sure our service is available in the injection system, in the IOC     

            //Now there's one more thing I want to do, and that's in the program.cs where I need to tell the builder
            //dot services wrapper that it needs to add a scoped method or a scoped instance of the service for authentication
            //state provider.
            //And then we're going to tell it that whenever you inject the authentication state provider, it can
            //also be implemented by our custom API authentication service provider.
            //So that way our little override will work properly.
            builder.Services.AddScoped<ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
                            provider.GetRequiredService<ApiAuthenticationStateProvider>());


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets(); // similar to app.UseStaticFiles() in .NET 6, but it’s optimized for Blazor’s new architecture
            app.MapRazorComponents<App>() // SignalR is automatically handled inside MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
