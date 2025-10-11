using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Middlewares;
using Jx.Cms.Plugin.Options;
using Jx.Cms.Plugin.Service.Both;
using Jx.Cms.Plugin.Service.Both.Impl;
using Jx.Cms.Plugin.Service.Admin;
using Jx.Cms.Plugin.Service.Admin.Impl;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Plugin.Service.Front.Impl;
using Jx.Cms.Plugin.Service;
using Jx.Cms.Plugin.Service.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jx.Cms.Plugin;

public class PluginStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureOptions<UiConfigureOptions>();
        
        // 注册服务
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IAdminMenuService, AdminMenuService>();
        services.AddScoped<Jx.Cms.Plugin.Service.Admin.IArticleService, Jx.Cms.Plugin.Service.Admin.Impl.ArticleService>();
        services.AddScoped<Jx.Cms.Plugin.Service.Admin.ICatalogService, Jx.Cms.Plugin.Service.Admin.Impl.CatalogService>();
        services.AddScoped<Jx.Cms.Plugin.Service.Admin.IPageService, Jx.Cms.Plugin.Service.Admin.Impl.PageService>();
        services.AddScoped<Jx.Cms.Plugin.Service.Front.IArticleService, Jx.Cms.Plugin.Service.Front.Impl.ArticleService>();
        services.AddScoped<Jx.Cms.Plugin.Service.Front.ICatalogService, Jx.Cms.Plugin.Service.Front.Impl.CatalogService>();
        services.AddScoped<Jx.Cms.Plugin.Service.Front.IPageService, Jx.Cms.Plugin.Service.Front.Impl.PageService>();
        services.AddScoped<IPaginationService, PaginationService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IPluginService, PluginService>();
        
        if (Util.IsInstalled) WidgetCache.UpdateCache();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<PluginMiddleware>();
    }
}