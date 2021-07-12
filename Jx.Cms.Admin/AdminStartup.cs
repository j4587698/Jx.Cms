using System;
using System.Reflection;
using Furion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Jx.Cms.Admin
{
    [AppStartup(-1)]
    public class AdminStartup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(op =>
            {
                op.CheckConsentNeeded = context => true;
                op.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(op =>
            {
                op.LoginPath = "/Admin/Login";
            });
            services.AddServerSideBlazor();
            services.AddBootstrapBlazor();
            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
                o.MaximumReceiveMessageSize = Int64.MaxValue;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new EmbeddedFileProvider(typeof(AdminStartup).Assembly, "Jx.Cms.Admin.wwwroot")
            });
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("install", "Install", "/Install/{controller}/{action}");
                endpoints.MapBlazorHub("~/Install/_blazor");
                endpoints.MapFallbackToAreaPage("~/Install/{*clientroutes:nonfile}","/_InstallHost", "Install");
                
                endpoints.MapAreaControllerRoute("admin", "Admin", "/Admin/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub("~/Admin/_blazor");
                endpoints.MapFallbackToAreaPage("~/Admin/{*clientroutes:nonfile}","/_AdminHost", "Admin");
            });
        }
    }
}