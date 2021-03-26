using Furion;
using Jx.Cms.Plugin.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jx.Cms.Plugin
{
    public class PluginStartup: AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureOptions<UiConfigureOptions>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            
        }
    }
}