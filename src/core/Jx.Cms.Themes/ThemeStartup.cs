using System.IO;
using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.Themes.Middlewares;
using Jx.Cms.Themes.Options;
using Jx.Cms.Themes.PartManager;
using Jx.Cms.Themes.RazorCompiler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

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
            
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
                options.ViewLocationExpanders.Add(new TemplateViewLocationExpander());
            });

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
            app.UseMiddleware<RedirectMiddleware>();
            app.UseMiddleware<RewriteMiddleware>();
        }
    }
}