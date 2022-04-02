using System.Linq;
using Furion;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Provider;
using Jx.Cms.Common.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;

namespace Jx.Cms.Common
{
    [AppStartup(int.MaxValue)]
    public class CommonStartup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ServicesExtension.Services = services;
            services.AddImageSharp().RemoveProvider<PhysicalFileSystemProvider>().AddProvider<ThemeProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseImageSharp();
            app.UseStaticFiles();
            
        }
    }
}