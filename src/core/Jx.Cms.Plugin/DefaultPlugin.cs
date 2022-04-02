using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Furion;
using Furion.Localization;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Plugin;
using McMaster.NETCore.Plugins;
using StackExchange.Profiling.Internal;
using PluginConfig = Jx.Cms.Common.Utils.PluginConfig;

namespace Jx.Cms.Plugin
{
    public static class DefaultPlugin
    {
        /// <summary>
        /// 已挂载插件列表
        /// </summary>
        private static readonly Dictionary<string, PluginLoader> Plugins = new();

        public static Assembly GetAssemblyByPluginId(string pluginId)
        {
            return Plugins.TryGetValue(pluginId, out var plugin) ? plugin.LoadDefaultAssembly() : null;
        }

        /// <summary>
        /// 挂载插件
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
            });
            Plugins.Add(pluginConfig.PluginId, plugin);
            AssemblyCache.AddAssembly(plugin.LoadDefaultAssembly());
        }

        public static void UnloadPlugin(PluginConfig pluginConfig)
        {
            if (Plugins.TryRemove(pluginConfig.PluginId, out PluginLoader plugin))
            {
                AssemblyCache.RemoveAssembly(plugin.LoadDefaultAssembly());
                plugin.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void ReLoadPlugin(PluginConfig pluginConfig)
        {
            if (Plugins.TryGetValue(pluginConfig.PluginId, out PluginLoader plugin))
            {
                plugin.Reload();
            }
        }
        
    }
}