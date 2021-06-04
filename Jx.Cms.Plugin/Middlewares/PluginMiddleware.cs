using System;
using System.Threading.Tasks;
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
            var plugins = DefaultPlugin.SystemPlugins;
            foreach (var plugin in plugins)
            {
                foreach (var type in plugin.Value)
                {
                    var instance = Activator.CreateInstance(type) as ISystemPlugin;
                    var ret = instance?.AddMiddleware(context);
                    if (ret != true)
                    {
                        return;
                    }
                }
            }
            await _next.Invoke(context);
        }
    }
}