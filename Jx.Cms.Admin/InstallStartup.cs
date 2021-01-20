using Furion;
using Jx.Cms.Admin.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jx.Cms.Admin
{
    [AppStartup(int.MaxValue - 10)]
    public class InstallStartup: AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {

        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<InstallMiddleware>("/Install/Step1");
        }
    }
}