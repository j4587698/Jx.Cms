using Jx.Cms.Common.Utils;
using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Install.Middlewares;

/// <summary>
///     安装判断中间件
/// </summary>
public class InstallMiddleware
{
    private readonly string _installPath;
    private readonly RequestDelegate _next;

    public InstallMiddleware(RequestDelegate next, string installPath)
    {
        _next = next;
        _installPath = installPath;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path != null && !Util.IsInstalled)
        {
            // 过滤静态文件和安装路径
            if (path.Contains(".js") || path.Contains(".css") || path.Contains(".png") ||
                path.Contains(".jpg") || path.Contains(".jpeg") || path.Contains(".gif") ||
                path.EndsWith(".ico") || path.EndsWith(".svg") || path.Contains("_blazor") ||
                path.Contains(_installPath))
                await _next.Invoke(context);
            else
                context.Response.Redirect(
                    $"{context.Request.Scheme}://{context.Request.Host}{_installPath}");
        }
        else
        {
            await _next.Invoke(context);
        }
    }
}