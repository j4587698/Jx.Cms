using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Jx.Cms.Common.Extensions;
using McMaster.NETCore.Plugins;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace Jx.Cms.Plugin
{
    public static class RazorPlugin
    {
        private static readonly Dictionary<string, PluginLoader> PluginLoaders = new Dictionary<string, PluginLoader>();

        /// <summary>
        /// 根据dll名字（不含扩展名）获取Assembly
        /// </summary>
        /// <param name="dllName"></param>
        /// <returns></returns>
        public static Assembly? GetAssemblyByDllName(string dllName)
        {
            var dllPath = PluginLoaders.Keys.FirstOrDefault(x => Path.GetFileName(x) == dllName);
            return dllPath.IsNullOrEmpty() ? null : PluginLoaders[dllPath].LoadDefaultAssembly();
        }

        /// <summary>
        /// 加载Razor插件
        /// </summary>
        /// <param name="dllPath"></param>
        /// <param name="partManager"></param>
        public static void LoadPlugin(string dllPath, ApplicationPartManager partManager)
        {
            if (PluginLoaders.ContainsKey(dllPath))
            {
                return;
            }
            var plugin = PluginLoader.CreateFromAssemblyFile(dllPath, config =>
            {
                config.IsUnloadable = true;
                config.PreferSharedTypes = true;
            });
            AddToPartManager(plugin, partManager);
            PluginLoaders.Add(dllPath, plugin);
        }

        private static void AddToPartManager(PluginLoader pluginLoader, ApplicationPartManager partManager)
        {
            var pluginAssembly = pluginLoader.LoadDefaultAssembly();
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
        /// <param name="dllPath"></param>
        /// <param name="partManager"></param>
        public static void RemovePlugin(string dllPath, ApplicationPartManager partManager)
        {
            if (!PluginLoaders.Remove(dllPath, out var plugin))
            {
                return;
            }
            RemoveFromPartManager(plugin, partManager);
            plugin.Dispose();
        }

        private static void RemoveFromPartManager(PluginLoader pluginLoader, ApplicationPartManager partManager)
        {
            var pluginAssembly = pluginLoader.LoadDefaultAssembly();
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
        /// <param name="dllPath"></param>
        public static void ReloadPlugin(string dllPath)
        {
            if (!PluginLoaders.TryGetValue(dllPath, out var plugin))
            {
                return;
            }
            plugin.Reload();
        }
    }
}