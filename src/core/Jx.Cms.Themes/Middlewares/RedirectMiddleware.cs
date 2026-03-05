using System.IO;
using Jx.Cms.Themes.Util;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Themes.Middlewares;

public class RedirectMiddleware
{
    private static readonly HashSet<string> StaticFileExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".js", ".css", ".map", ".ico", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp", ".bmp", ".avif", ".woff",
        ".woff2", ".ttf", ".eot", ".otf", ".json", ".txt", ".xml", ".webmanifest", ".wasm"
    };

    private readonly RequestDelegate _next;

    public RedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!(HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method)))
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.Value ?? string.Empty;
        if (ShouldSkip(path))
        {
            await _next(context);
            return;
        }

        var redirectPath = ThemeUtil.Redirect();
        if (redirectPath.IsNullOrEmpty())
            await _next(context);
        else
            context.Response.Redirect(redirectPath);
    }

    private static bool ShouldSkip(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path == "/") return false;

        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Install", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/_blazor", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/_content", StringComparison.OrdinalIgnoreCase))
            return true;

        if (!Path.HasExtension(path)) return false;
        return StaticFileExtensions.Contains(Path.GetExtension(path));
    }
}
