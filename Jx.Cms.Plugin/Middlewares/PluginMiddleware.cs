using System;
using System.Threading.Tasks;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Plugin;
using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Plugin.Middlewares
{
    public class PluginMiddleware
    {
        private readonly RequestDelegate _next;
        
        public PluginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var plugins = SystemPluginCache.GetSystemPlugins();
            foreach (var plugin in plugins)
            {
                var ret = plugin?.AddMiddleware(context);
                if (ret != true)
                {
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}