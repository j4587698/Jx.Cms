using System.Reflection;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Plugin.Plugin;
using Jx.Cms.Plugin.Utils;
using Microsoft.Extensions.Logging;

namespace Jx.Cms.Plugin.Cache;

public class SystemPluginCache : IPluginCache
{
    private static IEnumerable<Type> _systemTypes;

    private static void LogHookException(object pluginInstance, string hookName, Exception ex)
    {
        var pluginName = pluginInstance?.GetType().Name ?? "UnknownPlugin";
        var message = $"插件 {pluginName} 在 {hookName} 执行失败：{ex.Message}";
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

    public static void UpdateType()
    {
        _systemTypes = AssemblyCache.TypeList.Where(x => typeof(ISystemPlugin).IsAssignableFrom(x) && !x.IsAbstract);
    }

    public static void RemoveAssembly(Assembly assembly)
    {
        var list = assembly.GetTypes().Where(x => typeof(ISystemPlugin).IsAssignableFrom(x) && !x.IsAbstract);
        foreach (var type in list)
        {
            var systemPlugin = PluginInstanceFactory.CreateInstance<ISystemPlugin>(type);
            try
            {
                systemPlugin?.PluginDisable();
            }
            catch (Exception ex)
            {
                LogHookException(systemPlugin, nameof(ISystemPlugin.PluginDisable), ex);
            }
        }
    }

    public static IEnumerable<ISystemPlugin> GetSystemPlugins()
    {
        return _systemTypes == null
            ? Array.Empty<ISystemPlugin>()
            : _systemTypes.Select(PluginInstanceFactory.CreateInstance<ISystemPlugin>).Where(x => x != null);
    }
}
