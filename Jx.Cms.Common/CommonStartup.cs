using Furion;
using Jx.Cms.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jx.Cms.Common
{
    public class CommonStartup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ServicesExtension.Services = services;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }
    }
}