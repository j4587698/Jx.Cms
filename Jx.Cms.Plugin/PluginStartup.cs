using Furion;
using Jx.Cms.Plugin.Middlewares;
using Jx.Cms.Plugin.Options;
using Jx.Cms.Plugin.Widgets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jx.Cms.Plugin
{
    [AppStartup(int.MaxValue - 100)]
    public class PluginStartup: AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureOptions<UiConfigureOptions>();
            WidgetCache.UpdateCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<PluginMiddleware>();
        }
    }
}