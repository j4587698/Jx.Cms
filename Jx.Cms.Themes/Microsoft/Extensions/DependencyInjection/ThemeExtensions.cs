using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Jx.Cms.Common.Configure;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Plugin;
using Jx.Cms.Themes;
using Jx.Cms.Themes.Middlewares;
using Jx.Cms.Themes.PartManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ThemeExtensions
    {
        private static string libraryPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "Test"));
        private static void AddRclSupport(IServiceCollection services)
        {
            var provider = new PhysicalFileProvider(libraryPath);

            void CallBack(object obj)
            {
                var partManager = services.GetSingletonInstanceOrNull<ApplicationPartManager>();
                var dirs = Directory.GetDirectories(libraryPath);
                foreach (var dir in dirs)
                {
                    var dllName = Path.GetFileName(dir) + ".dll";
                    var dllPath = Path.Combine(dir, dllName);
                    if (File.Exists(dllPath))
                    {
                        RazorPlugin.LoadPlugin(dllPath, partManager);
                    }
                }

                MyActionDescriptorChangeProvider.Instance.HasChanged = true;
                MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                var viewCompiler = Configure.ServiceProvider.GetService<IViewCompilerProvider>() as MyViewCompilerProvider;
                viewCompiler?.Modify();
                provider.Watch("**").RegisterChangeCallback(CallBack, null);
            }

            provider.Watch("**").RegisterChangeCallback(CallBack, null);
        }

        public static IServiceCollection AddTheme(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddRazorPages(options =>
            {
                options.Conventions.Add(new ResponsivePageRouteModelConvention());
            }).ConfigureApplicationPartManager(manager  =>
            {
                var dirs = Directory.GetDirectories(libraryPath);
                foreach (var dir in dirs)
                {
                    var dllName = Path.GetFileName(dir) + ".dll";
                    var dllPath = Path.Combine(dir, dllName);
                    if (File.Exists(dllPath))
                    {
                        RazorPlugin.LoadPlugin(dllPath, manager);
                    }
                }
            });

            services.AddSingleton<MatcherPolicy, ResponsivePageMatcherPolicy>();
            services.AddSingleton<IActionDescriptorChangeProvider>(MyActionDescriptorChangeProvider.Instance);
            services.AddSingleton(MyActionDescriptorChangeProvider.Instance);
            services.Replace<IViewCompilerProvider, MyViewCompilerProvider>();
            AddRclSupport(services);
            return services;
        }
        
        public static IApplicationBuilder UseTheme(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            HttpContext2.Configure(httpContextAccessor);
            Utils.InitThemePath();
            app.UseMiddleware<RedirectMiddleware>();
            return app;
        }
        
        public static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection services)
            where TImplementation : TService
        {
            return services.Replace<TService>(typeof(TImplementation));
        }

        public static IServiceCollection Replace<TService>(this IServiceCollection services, Type implementationType)
        {
            return services.Replace(typeof(TService), implementationType);
        }

        public static IServiceCollection Replace(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (!services.TryGetDescriptors(serviceType, out var descriptors))
            {
                throw new ArgumentException($"No services found for {serviceType.FullName}.", nameof(serviceType));
            }

            foreach (var descriptor in descriptors)
            {
                var index = services.IndexOf(descriptor);

                services.Insert(index, descriptor.WithImplementationType(implementationType));

                services.Remove(descriptor);
            }

            return services;
        }

        private static bool TryGetDescriptors(this IServiceCollection services, Type serviceType, out ICollection<ServiceDescriptor> descriptors)
        {
            return (descriptors = services.Where(service => service.ServiceType == serviceType).ToArray()).Any();
        }

        private static ServiceDescriptor WithImplementationType(this ServiceDescriptor descriptor, Type implementationType)
        {
            return new ServiceDescriptor(descriptor.ServiceType, implementationType, descriptor.Lifetime);
        }
    }
}