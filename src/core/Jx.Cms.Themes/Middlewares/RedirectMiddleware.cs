using Jx.Cms.Themes.Util;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Themes.Middlewares;

public class RedirectMiddleware
{
    private readonly RequestDelegate _next;

    public RedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var hasArea = context.Request.RouteValues.ContainsKey("area");
        var path = context.Request.Path.ToString();

        // 过滤静态文件和Blazor资源
        if (path.EndsWith(".js") || path.EndsWith(".css") || path.EndsWith(".png") ||
            path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".gif") ||
            path.EndsWith(".ico") || path.EndsWith(".svg") || path.Contains("_blazor") ||
            path.StartsWith("/lib/") || path.StartsWith("/css/") || path.StartsWith("/js/") ||
            path.StartsWith("/_framework/"))
        {
            await _next.Invoke(context);
            return;
        }

        if (!hasArea)
        {
            var redirectPath = ThemeUtil.Redirect();
            if (redirectPath.IsNullOrEmpty())
                await _next.Invoke(context);
            else
                context.Response.Redirect(redirectPath);
        }
        else
        {
            await _next.Invoke(context);
        }
    }
}