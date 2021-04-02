using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Furion;
using Furion.Localization;
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

        /// <summary>
        /// 文章插件列表
        /// </summary>
        internal static readonly Dictionary<string, List<Type>> ArticlePlugins = new();

        /// <summary>
        /// 系统相关插件列表
        /// </summary>
        internal static readonly Dictionary<string, List<Type>> SystemPlugins = new();

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
            var types = plugin.LoadDefaultAssembly().GetTypes();
            // 文章相关插件列表
            var articleList = new List<Type>();
            foreach (var article in types.Where(x => typeof(IArticlePlugin).IsAssignableFrom(x) && !x.IsAbstract))
            {
                articleList.Add(article);
            }

            if (articleList.Count > 0)
            {
                ArticlePlugins.Add(pluginConfig.PluginId, articleList);
            }
            
            // 系统相关插件列表
            var systemList = new List<Type>();
            systemList.AddRange(types.Where(x => typeof(ISystemPlugin).IsAssignableFrom(x) && !x.IsAbstract).ToList());

            if (systemList.Count > 0)
            {
                SystemPlugins.Add(pluginConfig.PluginId, systemList);
            }
        }

        public static void UnloadPlugin(PluginConfig pluginConfig)
        {
            if (Plugins.TryRemove(pluginConfig.PluginId, out PluginLoader plugin))
            {
                ArticlePlugins.TryRemove(pluginConfig.PluginId, out _);
                SystemPlugins.TryRemove(pluginConfig.PluginId, out _);
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