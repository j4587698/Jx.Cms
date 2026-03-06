using System.Reflection;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Plugin;
using Jx.Cms.Plugin.Utils;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Logging;
using PluginConfig = Jx.Cms.Common.Utils.PluginConfig;

namespace Jx.Cms.Plugin;

public static class DefaultPlugin
{
    /// <summary>
    ///     已挂载插件列表
    /// </summary>
    private static readonly Dictionary<string, PluginLoader> Plugins = new();

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

    public static Assembly GetAssemblyByPluginId(string pluginId)
    {
        return Plugins.TryGetValue(pluginId, out var plugin) ? plugin.LoadDefaultAssembly() : null;
    }

    /// <summary>
    ///     挂载插件
    /// </summary>
    /// <param name="pluginConfig">插件信息</param>
    public static void LoadPlugin(PluginConfig pluginConfig)
    {
        if (Plugins.ContainsKey(pluginConfig.PluginId))
        {
            ReLoadPlugin(pluginConfig);
            return;
        }

        var plugin = PluginLoader.CreateFromAssemblyFile(pluginConfig.PluginPath, configure =>
        {
            configure.IsUnloadable = true;
            configure.PreferSharedTypes = true;
            configure.LoadInMemory = true;
        });
        Plugins.Add(pluginConfig.PluginId, plugin);

        var assembly = plugin.LoadDefaultAssembly();
        AssemblyCache.AddAssembly(assembly);
        InvokeLifecycle(assembly, x => x.PluginEnable(), nameof(ISystemPlugin.PluginEnable));
    }

    public static void InvokePluginDeleted(PluginConfig pluginConfig)
    {
        if (pluginConfig.PluginPath == null) return;

        if (Plugins.TryGetValue(pluginConfig.PluginId, out var plugin))
        {
            InvokeLifecycle(plugin.LoadDefaultAssembly(), x => x.PluginDeleted(), nameof(ISystemPlugin.PluginDeleted));
            return;
        }

        PluginLoader tempLoader = null;
        try
        {
            tempLoader = PluginLoader.CreateFromAssemblyFile(pluginConfig.PluginPath, configure =>
            {
                configure.IsUnloadable = true;
                configure.PreferSharedTypes = true;
                configure.LoadInMemory = true;
            });
            InvokeLifecycle(tempLoader.LoadDefaultAssembly(), x => x.PluginDeleted(), nameof(ISystemPlugin.PluginDeleted));
        }
        finally
        {
            tempLoader?.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public static void UnloadPlugin(PluginConfig pluginConfig)
    {
        if (!Plugins.ContainsKey(pluginConfig.PluginId)) return;

        var plugin = Plugins[pluginConfig.PluginId];
        Plugins.Remove(pluginConfig.PluginId);
        AssemblyCache.RemoveAssembly(plugin.LoadDefaultAssembly());
        plugin.Dispose();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public static void ReLoadPlugin(PluginConfig pluginConfig)
    {
        if (Plugins.TryGetValue(pluginConfig.PluginId, out var plugin)) plugin.Reload();
    }

    private static void InvokeLifecycle(Assembly assembly, Action<ISystemPlugin> hookAction, string hookName)
    {
        var lifecycleTypes = assembly.GetTypes().Where(x => typeof(ISystemPlugin).IsAssignableFrom(x) && !x.IsAbstract);
        foreach (var type in lifecycleTypes)
        {
            var plugin = PluginInstanceFactory.CreateInstance<ISystemPlugin>(type);
            if (plugin == null) continue;
            try
            {
                hookAction(plugin);
            }
            catch (Exception ex)
            {
                LogHookException(plugin, hookName, ex);
            }
        }
    }
}
