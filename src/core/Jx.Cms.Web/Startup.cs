using Jx.Cms.Common;
using Jx.Cms.Common.Exceptions;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext;
using Jx.Cms.Install;
using Jx.Cms.Plugin;
using Jx.Cms.Plugin.Service.Both;
using Jx.Cms.Themes;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.RateLimiting;

namespace Jx.Cms.Web;

public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
    public IConfiguration Configuration { get; } = configuration;
    public IWebHostEnvironment Environment { get; } = environment;

    public void ConfigureServices(IServiceCollection services)
    {

        // 1. DbStartup services (int.MaxValue - 50)
        var dbStartup = new DbStartup(Configuration);
        dbStartup.ConfigureServices(services);
        var dbConfig = Configuration.GetSection("Db").Get<DbConfig>();
        if (dbConfig != null)
        {
            var ret = DbStartup.SetupDb(services, dbConfig, Environment.IsDevelopment());
            if (!ret.isSuccess) throw new DbException(ret.msg);
            services.Configure<DbConfig>(x => x.CopyFrom(dbConfig));
        }
        else if (Util.IsInstalled)
        {
            throw new DbException("数据库配置错误，无数据库配置信息！");
        }
        
        // 2. PluginStartup services (int.MaxValue - 100)
        var pluginStartup = new PluginStartup();
        pluginStartup.ConfigureServices(services);

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
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.Configure<CookiePolicyOptions>(op =>
        {
            op.CheckConsentNeeded = context => true;
            op.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(op =>
        {
            op.LoginPath = "/Admin/Login";
        });
        services.AddBootstrapBlazor();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("search", context =>
            {
                var searchRateLimitPerMinute = 5;
                var settingsService = context.RequestServices.GetService<ISettingsService>();
                if (settingsService != null)
                {
                    var dbValue = settingsService.GetValue(nameof(SystemSettingsVm.SearchRateLimitPerMinute));
                    if (int.TryParse(dbValue, out var parsed) && parsed > 0)
                        searchRateLimitPerMinute = Math.Clamp(parsed, 1, 100);
                }

                var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var partitionKey = $"{remoteIp}:{searchRateLimitPerMinute}";
                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = searchRateLimitPerMinute,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });
        });
        services.AddSignalR(o =>
        {
            o.EnableDetailedErrors = true;
            o.MaximumReceiveMessageSize = long.MaxValue;
            o.DisableImplicitFromServicesParameters = true;
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
        app.UseRateLimiter();
        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
