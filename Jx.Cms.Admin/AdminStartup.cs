using System.Reflection;
using Furion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Jx.Cms.Admin
{
    [AppStartup(-1)]
    public class AdminStartup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServerSideBlazor();
            services.AddBootstrapBlazor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("admin", "Admin", "/Admin/{controller}/{action}");
                endpoints.MapBlazorHub("~/Admin/_blazor");
                endpoints.MapFallbackToAreaPage("~/Admin/{*clientroutes:nonfile}","/_AdminHost", "Admin");
            });
        }
    }
}