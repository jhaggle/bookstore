using System.ComponentModel;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Serilog;

namespace BookStoreApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // context is basically a wrapper around all the config info (like appsettings.json, environment variables, etc.)
            // context is a HostBuilderContext (contains app configuration, environment, etc.)
            //loggingConfiguration is a LoggerConfiguration (Serilog’s configuration object)
            // Below we are passing an inline function as a parameter to UseSerilog(...)
            // Aha, detta gör att 
            // 1) Första sättet
            builder.Host.UseSerilog((context, loggingConfiguration) => 
            loggingConfiguration.WriteTo.Console().ReadFrom.Configuration(context.Configuration));


            // 2) Andra sättet
            builder.Host.UseSerilog((context, loggingConfiguration) =>
            {
                // directly instructs Serilog to send logs to the Console.
                loggingConfiguration.WriteTo.Console();
                // Also, look in appsettings.json (or other config sources) for any Serilog section, and apply those instructions too
                loggingConfiguration.ReadFrom.Configuration(context.Configuration);
            });


            // 3) Tredje sättet
            static void ConfigureSerilog(HostBuilderContext context, LoggerConfiguration loggingConfiguration)
            {
                loggingConfiguration.WriteTo.Console();
                loggingConfiguration.ReadFrom.Configuration(context.Configuration);
            }
            // Host is a property. Properties in C# can return objects, and we can call methods on those objects
            //A “property” is simply a named member of a class that can get(and sometimes set) a value—and that value can be anything, including a complex object like an IHostBuilder.
            builder.Host.UseSerilog(ConfigureSerilog);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllStuff", 
                    b => b.AllowAnyMethod().
                            AllowAnyHeader().
                            AllowAnyOrigin()
                    );
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAllStuff"); // We have now created a nice policy that will allow any client from anywhere to access. 
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
