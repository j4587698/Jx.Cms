using System.IO;
using Furion;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin;
using Jx.Cms.Themes.Middlewares;
using Jx.Cms.Themes.Options;
using Jx.Cms.Themes.PartManager;
using Jx.Cms.Themes.Util;
using Masuit.Tools.Core.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace Jx.Cms.Themes
{
    public class ThemeStartup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            if (!Directory.Exists(Constants.ThemePath))
            {
                Directory.CreateDirectory(Constants.ThemePath);
            }
            
            services.AddStaticHttpContext();

            services.AddRazorPages(options =>
            {
                options.Conventions.Add(new ResponsivePageRouteModelConvention());
            });

            services.AddSingleton<MatcherPolicy, ResponsivePageMatcherPolicy>();
            services.AddSingleton<IActionDescriptorChangeProvider>(MyActionDescriptorChangeProvider.Instance);
            services.AddSingleton(MyActionDescriptorChangeProvider.Instance);
            services.Replace<IViewCompilerProvider, MyViewCompilerProvider>();
            services.ConfigureOptions<UiConfigureOptions>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticHttpContext();
            app.UseMiddleware<RedirectMiddleware>();
        }
    }
}