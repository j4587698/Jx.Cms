using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Jx.Cms.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "config")))
            {
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "config"));
            }
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Configure Serilog
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));
            
            // Create and configure startup
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
            
            var app = builder.Build();
            
            // Configure middleware
            var env = app.Environment;
            startup.Configure(app, env);
            
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Application configured. Starting to listen...");
            
            app.Run();
        }
    }
}