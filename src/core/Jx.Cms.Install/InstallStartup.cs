﻿using Furion;
using Jx.Cms.Install.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jx.Cms.Install;

[AppStartup(int.MaxValue - 20)]
public class InstallStartup : Furion.AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseMiddleware<InstallMiddleware>("/Install/Step1");
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapRazorPages();
            endpoints.MapFallbackToPage("~/Install/{*clientroutes:nonfile}", "/_InstallHost");
        });
    }
}