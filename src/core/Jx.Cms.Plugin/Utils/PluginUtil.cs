using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BootstrapBlazor.Components;
using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.DbContext.Entities.Settings;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Console = System.Console;
using Constants = Jx.Cms.Common.Utils.Constants;

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
            if (!Directory.Exists(Constants.PluginPath))
            {
                Directory.CreateDirectory(Constants.PluginPath);
                return pluginConfigs;
            }
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
            Directory.Delete(Path.GetDirectoryName(plugin.PluginPath)!, true);
            return true;
        }

        /// <summary>
        /// 文章页展示时执行
        /// </summary>
        /// <param name="articleModel"></param>
        /// <returns></returns>
        public static ArticleModel OnArticleShow(ArticleModel articleModel)
        {
            foreach (var instance in ArticlePluginCache.GetArticlePlugins())
            {
                instance?.OnArticleShow(articleModel);
            }
            return articleModel;
        }

        /// <summary>
        /// 文章列表显示时执行
        /// </summary>
        /// <returns></returns>
        public static List<EditorExtModel> OnArticleEditorShow(IEnumerable<IArticlePlugin> articlePlugins)
        {
            var extModels = new List<EditorExtModel>();
            foreach (var instance in articlePlugins)
            {
                var ret = instance?.AddEditorToolbarButton(App.GetService<DialogService>());
                if (ret != null)
                {
                    extModels.Add(ret);
                }
            }

            return extModels;
        }

        /// <summary>
        /// 获取底部扩展内容
        /// </summary>
        /// <param name="articlePlugins"></param>
        /// <param name="articleEntity"></param>
        /// <returns></returns>
        public static List<ArticleExtModel> GetBottomExtModels(IEnumerable<IArticlePlugin> articlePlugins, ArticleEntity articleEntity)
        {
            var articleExtModels = new List<ArticleExtModel>();
            foreach (var articlePlugin in articlePlugins)
            {
                var ret = articlePlugin?.BottomExt(articleEntity);
                if (ret != null)
                {
                    articleExtModels.AddRange(ret);
                }
            }
            return articleExtModels;
        }
        
        /// <summary>
        /// 获取右侧扩展内容
        /// </summary>
        /// <param name="articlePlugins"></param>
        /// <param name="articleEntity"></param>
        /// <returns></returns>
        public static List<ArticleExtModel> GetRightExtModels(IEnumerable<IArticlePlugin> articlePlugins, ArticleEntity articleEntity)
        {
            var articleExtModels = new List<ArticleExtModel>();
            foreach (var articlePlugin in articlePlugins)
            {
                var ret = articlePlugin?.RightExt(articleEntity);
                if (ret != null)
                {
                    articleExtModels.AddRange(ret);
                }
            }
            return articleExtModels;
        }

        public static List<PluginMenuModel> OnMenuShow()
        {
            var menuModel = new List<PluginMenuModel>();
            foreach (var instance in SystemPluginCache.GetSystemPlugins())
            {
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