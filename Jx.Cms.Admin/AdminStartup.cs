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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles(new StaticFileOptions()
            {
                
                FileProvider = new EmbeddedFileProvider(typeof(AdminStartup).Assembly, "Jx.Cms.Admin.wwwroot")
            });
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToAreaPage("/_AdminHost", "Admin");
            });
        }
    }
}