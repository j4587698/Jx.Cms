using System.IO;
using Furion;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Plugin;
using Jx.Cms.Themes.Middlewares;
using Jx.Cms.Themes.Options;
using Jx.Cms.Themes.PartManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        private static readonly string LibraryPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "Theme"));

        private static void LoadPlugin(ApplicationPartManager partManager)
        {
            var dirs = Directory.GetDirectories(LibraryPath);
            foreach (var dir in dirs)
            {
                var dllName = Path.GetFileName(dir) + ".dll";
                var dllPath = Path.Combine(dir, dllName);
                if (File.Exists(dllPath))
                {
                    RazorPlugin.LoadPlugin(dllPath, partManager);
                }
            }
            Utils.PathDllDic.Clear();
            ViewsFeature viewsFeature = new ViewsFeature();
            partManager.PopulateFeature(viewsFeature);
            foreach (var view in viewsFeature.ViewDescriptors)
            {
                if (Utils.PathDllDic.ContainsKey(view.RelativePath))
                {
                    continue;
                }
                var name = view.Item.Type.Assembly.ManifestModule.Name.Replace(".Views", "");
                // if (name == "Jx.Cms.Web.dll")
                // {
                //     continue;
                // }
                Utils.PathDllDic.Add(view.RelativePath, name);
            }
        }

        private static void AddRclSupport(IServiceCollection services)
        {
            var provider = new PhysicalFileProvider(LibraryPath);

            void CallBack(object obj)
            {
                var partManager = services.GetSingletonInstanceOrNull<ApplicationPartManager>();
                LoadPlugin(partManager);

                MyActionDescriptorChangeProvider.Instance.HasChanged = true;
                MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                var viewCompiler = App.GetService<IViewCompilerProvider>() as MyViewCompilerProvider;
                viewCompiler?.Modify();
                provider.Watch("**").RegisterChangeCallback(CallBack, null);
            }

            provider.Watch("**").RegisterChangeCallback(CallBack, null);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRazorPages(options =>
            {
                options.Conventions.Add(new ResponsivePageRouteModelConvention());
            }).ConfigureApplicationPartManager(LoadPlugin);

            services.AddSingleton<MatcherPolicy, ResponsivePageMatcherPolicy>();
            services.AddSingleton<IActionDescriptorChangeProvider>(MyActionDescriptorChangeProvider.Instance);
            services.AddSingleton(MyActionDescriptorChangeProvider.Instance);
            services.Replace<IViewCompilerProvider, MyViewCompilerProvider>();
            AddRclSupport(services);
            services.ConfigureOptions<UiConfigureOptions>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            HttpContext2.Configure(httpContextAccessor);
            Utils.InitThemePath();
            app.UseMiddleware<RedirectMiddleware>();
        }
    }
}