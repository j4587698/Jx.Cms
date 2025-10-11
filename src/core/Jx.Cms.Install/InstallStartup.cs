using Jx.Cms.Install.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jx.Cms.Install;

public class InstallStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseMiddleware<InstallMiddleware>("/Install");
    }
}