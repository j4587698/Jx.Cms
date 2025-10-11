using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Jx.Cms.Common;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext;
using Jx.Cms.Install;
using Jx.Cms.Plugin;
using Jx.Cms.Themes;
using System;

namespace Jx.Cms.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // 1. PluginStartup services (int.MaxValue - 100)
            var pluginStartup = new PluginStartup();
            pluginStartup.ConfigureServices(services);

            // 2. DbStartup services (int.MaxValue - 50)
            var dbStartup = new DbStartup(Configuration);
            dbStartup.ConfigureServices(services);
            var dbConfig = Configuration.GetSection("Db").Get<DbConfig>();
            if (dbConfig != null)
            {
                var ret = DbStartup.SetupDb(services, dbConfig);
                if (!ret.isSuccess)
                {
                    throw new Common.Exceptions.DbException(ret.msg);
                }
                services.Configure<DbConfig>(x => Jx.Cms.Common.Extensions.ObjectExtension.CopyFrom(x, dbConfig));
            }
            else if (Util.IsInstalled)
            {
                throw new Common.Exceptions.DbException("数据库配置错误，无数据库配置信息！");
            }

            // 3. InstallStartup services (int.MaxValue - 20)
            var installStartup = new InstallStartup();
            installStartup.ConfigureServices(services);

            // 4. CommonStartup services (int.MaxValue)
            var commonStartup = new CommonStartup();
            commonStartup.ConfigureServices(services);

            // 5. ThemeStartup services (default)
            var themeStartup = new ThemeStartup();
            themeStartup.ConfigureServices(services);

            // 原始服务配置
            services.AddControllersWithViews();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped<HttpClient>();
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
            // 设置服务提供器
            ServicesExtension.ServiceProvider = app.ApplicationServices;

            // 1. PluginStartup middleware
            var pluginStartup = new PluginStartup();
            pluginStartup.Configure(app, env);

            // 2. ThemeStartup middleware
            var themeStartup = new ThemeStartup();
            themeStartup.Configure(app, env);

            // 3. CommonStartup middleware
            var commonStartup = new CommonStartup();
            commonStartup.Configure(app, env);

            // 4. InstallStartup middleware
            var installStartup = new InstallStartup();
            installStartup.Configure(app, env);

            // 原始中间件配置
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            
            // Admin endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("admin", "Admin", "/Admin/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub("~/_blazor");
                endpoints.MapFallbackToPage("~/Admin/{*clientroutes:nonfile}","/Admin/_AdminHost");
            });
        }
    }
}