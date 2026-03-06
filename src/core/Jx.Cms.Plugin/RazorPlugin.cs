using System.Reflection;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.Cache;
using McMaster.NETCore.Plugins;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Jx.Cms.Plugin;

public static class RazorPlugin
{
    private sealed class ThemePluginState
    {
        public PluginLoader Loader { get; init; }
        public Assembly Assembly { get; set; }
        public HashSet<string> PartNames { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private static readonly Dictionary<ThemeType, ThemePluginState> PluginLoaders = new();

    /// <summary>
    ///     根据主题类型（不含扩展名）获取Assembly
    /// </summary>
    /// <param name="themeType">类型</param>
    /// <returns></returns>
    public static Assembly GetAssemblyByThemeType(ThemeType themeType)
    {
        return PluginLoaders.TryGetValue(themeType, out var pluginState) ? pluginState.Assembly : null;
    }

    /// <summary>
    ///     加载Razor插件
    /// </summary>
    /// <param name="themeConfig"></param>
    /// <param name="partManager"></param>
    public static void LoadPlugin(ThemeConfig themeConfig, ApplicationPartManager partManager)
    {
        if (PluginLoaders.ContainsKey(themeConfig.ThemeType)) RemovePlugin(themeConfig, partManager);
        var plugin = PluginLoader.CreateFromAssemblyFile(
            Path.Combine(themeConfig.Path, $"{Path.GetFileName(themeConfig.Path)}.dll"), config =>
            {
                config.IsUnloadable = true;
                config.PreferSharedTypes = true;
                config.LoadInMemory = true;
            });
        var assembly = plugin.LoadDefaultAssembly();
        var partNames = AddToPartManager(plugin, partManager, assembly);
        PluginLoaders.Add(themeConfig.ThemeType, new ThemePluginState
        {
            Loader = plugin,
            Assembly = assembly,
            PartNames = partNames
        });
        AssemblyCache.AddAssembly(assembly);
    }

    private static HashSet<string> AddToPartManager(PluginLoader pluginLoader, ApplicationPartManager partManager,
        Assembly pluginAssembly)
    {
        var partNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (partManager == null) return partNames;

        void AddAssemblyParts(Assembly assembly)
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
            foreach (var applicationPart in partFactory.GetApplicationParts(assembly))
            {
                partManager.ApplicationParts.Add(applicationPart);
                partNames.Add(applicationPart.Name);
            }
        }

        AddAssemblyParts(pluginAssembly);
        var relatedAssembliesAttrs = pluginAssembly.GetCustomAttributes<RelatedAssemblyAttribute>();
        foreach (var attr in relatedAssembliesAttrs)
        {
            var assembly = pluginLoader.LoadAssembly(attr.AssemblyFileName);
            AddAssemblyParts(assembly);
        }

        return partNames;
    }

    /// <summary>
    ///     移除Razor插件
    /// </summary>
    /// <param name="themeConfig"></param>
    /// <param name="partManager"></param>
    public static void RemovePlugin(ThemeConfig themeConfig, ApplicationPartManager partManager)
    {
        if (PluginLoaders.Remove(themeConfig.ThemeType, out var pluginState))
        {
            AssemblyCache.RemoveAssembly(pluginState.Assembly);
            RemoveFromPartManager(partManager, pluginState.PartNames);
            pluginState.Loader.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    private static void RemoveFromPartManager(ApplicationPartManager partManager, IReadOnlyCollection<string> partNames)
    {
        if (partManager == null || partNames == null || partNames.Count == 0) return;
        foreach (var partName in partNames)
        {
            var parts = partManager.ApplicationParts.Where(x => x.Name == partName).ToArray();
            foreach (var part in parts) partManager.ApplicationParts.Remove(part);
        }
    }

    /// <summary>
    ///     重新载入Plugin
    /// </summary>
    /// <param name="themeType"></param>
    public static void ReloadPlugin(ThemeType themeType)
    {
        if (!PluginLoaders.TryGetValue(themeType, out var pluginState)) return;
        pluginState.Loader.Reload();
        pluginState.Assembly = pluginState.Loader.LoadDefaultAssembly();
    }
}
