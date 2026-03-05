using Jx.Cms.Common.Extensions;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Plugin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Jx.Cms.Plugin.Middlewares;

public class PluginMiddleware
{
    private readonly RequestDelegate _next;

    private static void LogHookException(object pluginInstance, Exception ex)
    {
        var pluginName = pluginInstance?.GetType().Name ?? "UnknownPlugin";
        var message = $"插件 {pluginName} 在 {nameof(ISystemPlugin.AddMiddleware)} 执行失败：{ex.Message}";
        try
        {
            ServicesExtension.GetService<ILoggerFactory>()
                ?.CreateLogger("Jx.Cms.Plugin.Hook")
                .LogWarning(ex, "{Message}", message);
        }
        catch
        {
            // 日志异常不影响主流程。
        }
    }

    public PluginMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var plugins = SystemPluginCache.GetSystemPlugins();
        foreach (var plugin in plugins)
        {
            try
            {
                var ret = plugin?.AddMiddleware(context);
                if (ret != true) return;
            }
            catch (Exception ex)
            {
                LogHookException(plugin, ex);
            }
        }

        await _next.Invoke(context);
    }
}
