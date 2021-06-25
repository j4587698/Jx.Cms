using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BootstrapBlazor.Components;
using Furion.DependencyInjection.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Cms.Entities.Article;
using Jx.Cms.Entities.Settings;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Masuit.Tools;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Jx.Cms.Plugin.Utils
{
    public static class PluginUtil
    {
        /// <summary>
        /// 获取所有的插件列表
        /// </summary>
        /// <returns></returns>
        public static List<PluginConfig> GetAllPlugins()
        {
            var dbPlugins = PluginEntity.Select.ToList();
            var pluginConfigs = new List<PluginConfig>();
            var dirs = Directory.GetDirectories(Constants.PluginPath);
            foreach (var dir in dirs)
            {
                var configPath = Path.Combine(dir, "plugin.json");
                var dllPath = Path.Combine(dir, Path.GetFileName(dir) + ".dll");
                if (File.Exists(configPath) && File.Exists(dllPath))
                {
                    var config = JsonConvert.DeserializeObject<PluginConfig>(File.ReadAllText(configPath));
                    if (dbPlugins.Any(x => x.PluginId == config.PluginId))
                    {
                        config.IsEnable = dbPlugins.First(x => x.PluginId == config.PluginId).IsEnable;
                    }
                    else
                    {
                        new PluginEntity {IsEnable = false, PluginId = config.PluginId}.Save();
                        config.IsEnable = false;
                    }
                    config.PluginPath = dllPath;
                    pluginConfigs.Add(config);
                }
            }
            return pluginConfigs;
        }

        /// <summary>
        /// 切换插件的启用状态
        /// </summary>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        public static bool ChangePluginStatus(string pluginId)
        {
            var plugin = GetAllPlugins().FirstOrDefault(x => x.PluginId == pluginId);
            if (plugin == null)
            {
                return false;
            }
            if (plugin.IsEnable)
            {
                if (PluginEntity.Select.Where(x => x.PluginId == pluginId).ToUpdate().Set(x => x.IsEnable, false)
                    .ExecuteAffrows() == 0)
                {
                    return false;
                }
                DefaultPlugin.UnloadPlugin(plugin);
            }
            else
            {
                if (PluginEntity.Select.Where(x => x.PluginId == pluginId).ToUpdate().Set(x => x.IsEnable, true)
                    .ExecuteAffrows() == 0)
                {
                    return false;
                }
                DefaultPlugin.LoadPlugin(plugin);
            }

            return true;
        }

        /// <summary>
        /// 删除插件（包括插件文件）
        /// </summary>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        public static bool DeletePlugin(string pluginId)
        {
            var plugin = GetAllPlugins().FirstOrDefault(x => x.PluginId == pluginId);
            if (plugin == null)
            {
                return false;
            }

            DefaultPlugin.UnloadPlugin(plugin);
            if (plugin.PluginPath.IsNullOrEmpty())
            {
                return false;
            }
            Directory.Delete(Path.GetDirectoryName(plugin.PluginPath), true);
            return true;
        }

        /// <summary>
        /// 文章页展示时执行
        /// </summary>
        /// <param name="articleModel"></param>
        /// <returns></returns>
        public static ArticleModel OnArticleShow(ArticleModel articleModel)
        {
            var articlePlugin = new List<Type>();
            DefaultPlugin.ArticlePlugins.ForEach(x => articlePlugin.AddRange(x.Value));
            foreach (var type in articlePlugin)
            {
                 var instance = Activator.CreateInstance(type) as IArticlePlugin;
                 instance?.OnArticleShow(articleModel);
            }
            return articleModel;
        }

        /// <summary>
        /// 文章列表显示时执行
        /// </summary>
        /// <returns></returns>
        public static List<EditorExtModel> OnArticleEditorShow()
        {
            var extModels = new List<EditorExtModel>();
            var articlePlugin = new List<Type>();
            DefaultPlugin.ArticlePlugins.ForEach(x => articlePlugin.AddRange(x.Value));
            foreach (var type in articlePlugin)
            {
                var instance = Activator.CreateInstance(type) as IArticlePlugin;
                var ret = instance?.AddEditorToolbarButton(ServiceProviderHelper.ServiceProvider.GetService<DialogService>());
                if (ret != null)
                {
                    extModels.Add(ret);
                }
            }

            return extModels;
        }

        public static List<PluginMenuModel> OnMenuShow()
        {
            var menuModel = new List<PluginMenuModel>();
            var menuPlugin = new List<Type>();
            DefaultPlugin.SystemPlugins.ForEach(x => menuPlugin.AddRange(x.Value));
            foreach (var plugin in menuPlugin)
            {
                var instance = Activator.CreateInstance(plugin) as ISystemPlugin;
                var ret = instance?.AddMenuItem();
                if (ret != null && ret.Count > 0)
                {
                    menuModel.AddRange(ret);
                }
            }

            PluginMenuModels = menuModel;
            return menuModel;
        }

        private static List<PluginMenuModel> PluginMenuModels { get; set; }

        public static RenderFragment OnPluginPageShow(string menuId)
        {
            return PluginMenuModels.FirstOrDefault(x => x.MenuId == menuId)?.PluginBody;
        }
    }
}