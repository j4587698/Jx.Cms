using System.IO;
using Jx.Cms.Rewrite;
using Jx.Cms.Themes.Model;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Themes.Middlewares;

public class RewriteMiddleware
{
    private static readonly HashSet<string> StaticFileExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".js", ".css", ".map", ".ico", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp", ".bmp", ".avif", ".woff",
        ".woff2", ".ttf", ".eot", ".otf", ".json", ".txt", ".xml", ".webmanifest", ".wasm"
    };

    private readonly RequestDelegate _next;

    public RewriteMiddleware(RequestDelegate next)
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

        var settings = RewriterModel.GetSettings();
        if (settings == null ||
            string.Equals(settings.RewriteOption, nameof(RewriteOptionEnum.Dynamic), StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var url = RewriteUtil.AnalysisArticle(path, settings);
        if (url != null)
        {
            context.Request.Path = "/Post";
            context.Request.QueryString = new QueryString(url);
            await _next(context);
            return;
        }

        url = RewriteUtil.AnalysisPage(path, settings);
        if (url != null)
        {
            context.Request.Path = "/Page";
            context.Request.QueryString = new QueryString(url);
            await _next(context);
            return;
        }

        url = RewriteUtil.AnalysisIndex(path, settings);
        if (url != null)
        {
            context.Request.Path = "/";
            context.Request.QueryString = new QueryString(url);
            await _next(context);
            return;
        }

        url = RewriteUtil.AnalysisCatalog(path, settings);
        if (url != null)
        {
            context.Request.Path = "/Catalog";
            context.Request.QueryString = new QueryString(url);
            await _next(context);
            return;
        }

        url = RewriteUtil.AnalysisTag(path, settings);
        if (url != null)
        {
            context.Request.Path = "/Tag";
            context.Request.QueryString = new QueryString(url);
            await _next(context);
            return;
        }

        url = RewriteUtil.AnalysisDate(path, settings);
        if (url != null)
        {
            context.Request.Path = "/Date";
            context.Request.QueryString = new QueryString(url);
            await _next(context);
            return;
        }

        await _next(context);
    }

    private static bool ShouldSkip(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path == "/") return false;

        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Install", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Search", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/_blazor", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/_content", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
            return true;

        if (!Path.HasExtension(path)) return false;
        return StaticFileExtensions.Contains(Path.GetExtension(path));
    }
}
