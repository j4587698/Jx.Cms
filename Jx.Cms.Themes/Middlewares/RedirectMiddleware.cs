using System.Threading.Tasks;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Themes.Middlewares
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;
        
        public RedirectMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var hasArea = context.Request.RouteValues.ContainsKey("area");
            
            if (!hasArea)
            {
                var path = ThemeUtil.Redirect();
                if (path.IsNullOrEmpty())
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.Redirect(path);
                }
                
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}