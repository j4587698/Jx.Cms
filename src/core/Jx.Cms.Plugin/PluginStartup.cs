using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Middlewares;
using Jx.Cms.Plugin.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jx.Cms.Plugin
{
    public class PluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureOptions<UiConfigureOptions>();
            if (Util.IsInstalled)
            {
                WidgetCache.UpdateCache();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<PluginMiddleware>();
        }
    }
}