using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jx.Cms.Common.Utils;
using Jx.Cms.Entities.Article;
using Jx.Cms.Entities.Settings;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Masuit.Tools;
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
        public static ArticleModel ArticleShow(ArticleModel articleModel)
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

        public static List<EditorExtModel> ArticleEditorShow()
        {
            var extModels = new List<EditorExtModel>();
            var articlePlugin = new List<Type>();
            DefaultPlugin.ArticlePlugins.ForEach(x => articlePlugin.AddRange(x.Value));
            foreach (var type in articlePlugin)
            {
                var instance = Activator.CreateInstance(type) as IArticlePlugin;
                var ret = instance?.AddEditorToolbarButton();
                if (ret != null)
                {
                    extModels.Add(ret);
                }
            }

            return extModels;
        }
    }
}