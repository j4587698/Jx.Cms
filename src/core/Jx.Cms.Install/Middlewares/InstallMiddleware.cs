using System.Threading.Tasks;
using Jx.Cms.Common.Utils;
using Microsoft.AspNetCore.Http;
using Jx.Toolbox.Extensions;

namespace Jx.Cms.Install.Middlewares
{
    /// <summary>
    /// 安装判断中间件
    /// </summary>
    public class InstallMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _installPath;

        public InstallMiddleware(RequestDelegate next, string installPath)
        {
            _next = next;
            _installPath = installPath;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value != null && !Util.IsInstalled &&
                !context.Request.Path.Value.Contains(new []{_installPath, ".js", ".css", "_blazor"}))
            {
                context.Response.Redirect(
                    $"{context.Request.Scheme}://{context.Request.Host}{_installPath}");
            }
            else
            {
                await _next.Invoke(context);
            }
            
        }
    }
}