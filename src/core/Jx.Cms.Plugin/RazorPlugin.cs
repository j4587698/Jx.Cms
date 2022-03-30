using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Plugin;
using McMaster.NETCore.Plugins;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using StackExchange.Profiling.Internal;

namespace Jx.Cms.Plugin
{
    public static class RazorPlugin
    {
        private static readonly Dictionary<ThemeType, PluginLoader> PluginLoaders = new Dictionary<ThemeType, PluginLoader>();

        /// <summary>
        /// 根据主题类型（不含扩展名）获取Assembly
        /// </summary>
        /// <param name="themeType">类型</param>
        /// <returns></returns>
        public static Assembly GetAssemblyByThemeType(ThemeType themeType)
        {
            return PluginLoaders.TryGetValue(themeType, out var pluginLoader) ? pluginLoader.LoadDefaultAssembly() : null;
        }

        /// <summary>
        /// 加载Razor插件
        /// </summary>
        /// <param name="themeConfig"></param>
        /// <param name="partManager"></param>
        public static void LoadPlugin(ThemeConfig themeConfig, ApplicationPartManager partManager)
        {
            if (PluginLoaders.ContainsKey(themeConfig.ThemeType))
            {
                RemovePlugin(themeConfig, partManager);
            }
            var plugin = PluginLoader.CreateFromAssemblyFile(Path.Combine(themeConfig.Path, $"{Path.GetFileName(themeConfig.Path)}.dll"), config =>
            {
                config.IsUnloadable = true;
                config.PreferSharedTypes = true;
            });
            var assembly = plugin.LoadDefaultAssembly();
            AddToPartManager(plugin, partManager, assembly);
            PluginLoaders.Add(themeConfig.ThemeType, plugin);
            AssemblyCache.AddAssembly(assembly);
        }

        private static void AddToPartManager(PluginLoader pluginLoader, ApplicationPartManager partManager, Assembly pluginAssembly)
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginAssembly);
            foreach (var applicationPart in partFactory.GetApplicationParts(pluginAssembly))
            {
                partManager.ApplicationParts.Add(applicationPart);
            }
            var relatedAssembliesAttrs = pluginAssembly.GetCustomAttributes<RelatedAssemblyAttribute>();
            foreach (var attr in relatedAssembliesAttrs)
            {
                var assembly = pluginLoader.LoadAssembly(attr.AssemblyFileName);
                partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
                foreach (var part in partFactory.GetApplicationParts(assembly))
                {
                    partManager.ApplicationParts.Add(part);
                }
            }
        }

        /// <summary>
        /// 移除Razor插件
        /// </summary>
        /// <param name="themeConfig"></param>
        /// <param name="partManager"></param>
        public static void RemovePlugin(ThemeConfig themeConfig, ApplicationPartManager partManager)
        {
            if (PluginLoaders.Remove(themeConfig.ThemeType, out var plugin))
            {
                var pluginAssembly = AssemblyCache.RemoveAssembly(plugin.LoadDefaultAssembly());
                RemoveFromPartManager(plugin, partManager, pluginAssembly);
                plugin.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static void RemoveFromPartManager(PluginLoader pluginLoader, ApplicationPartManager partManager, Assembly pluginAssembly)
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginAssembly);
            foreach (var applicationPart in partFactory.GetApplicationParts(pluginAssembly))
            {
                var parts = partManager.ApplicationParts.Where(x => x.Name == applicationPart.Name).ToArray();
                foreach (var part in parts)
                {
                    partManager.ApplicationParts.Remove(part);
                }
            }
            var relatedAssembliesAttrs = pluginAssembly.GetCustomAttributes<RelatedAssemblyAttribute>();
            foreach (var attr in relatedAssembliesAttrs)
            {
                var assembly = pluginLoader.LoadAssembly(attr.AssemblyFileName);
                partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
                foreach (var applicationPart in partFactory.GetApplicationParts(assembly))
                {
                    var parts = partManager.ApplicationParts.Where(x => x.Name == applicationPart.Name).ToArray();
                    foreach (var part in parts)
                    {
                        partManager.ApplicationParts.Remove(part);
                    }
                }
            }
        }

        /// <summary>
        /// 重新载入Plugin
        /// </summary>
        /// <param name="themeType"></param>
        public static void ReloadPlugin(ThemeType themeType)
        {
            if (!PluginLoaders.TryGetValue(themeType, out var plugin))
            {
                return;
            }
            plugin.Reload();
        }
    }
}