using BootstrapBlazor.Components;
using Jx.Cms.Common.Utils;
using Jx.Cms.Common.Extensions;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.DbContext.Entities.Settings;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Constants = Jx.Cms.Common.Utils.Constants;

namespace Jx.Cms.Plugin.Utils;

public static class PluginUtil
{
    /// <summary>
    ///     插件变更通知（用于刷新静态文件提供器等运行时状态）
    /// </summary>
    public static Action<PluginConfig> PluginModify;

    private static List<PluginMenuModel> PluginMenuModels { get; set; }

    private static void NotifyPluginModify(PluginConfig pluginConfig)
    {
        try
        {
            PluginModify?.Invoke(pluginConfig);
        }
        catch (Exception ex)
        {
            HandleHookException(pluginConfig, nameof(PluginModify), ex);
        }
    }

    private static void HandleHookException(object pluginInstance, string hookName, Exception ex,
        Action<string> onWarning = null)
    {
        var pluginName = pluginInstance?.GetType().Name ?? "UnknownPlugin";
        var message = $"插件 {pluginName} 在 {hookName} 执行失败：{ex.Message}";
        onWarning?.Invoke(message);
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

    /// <summary>
    ///     获取所有的插件列表
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
                    new PluginEntity { IsEnable = false, PluginId = config.PluginId }.Save();
                    config.IsEnable = false;
                }

                config.PluginPath = dllPath;
                pluginConfigs.Add(config);
            }
        }

        return pluginConfigs;
    }

    /// <summary>
    ///     切换插件的启用状态
    /// </summary>
    /// <param name="pluginId"></param>
    /// <returns></returns>
    public static bool ChangePluginStatus(string pluginId)
    {
        var plugin = GetAllPlugins().FirstOrDefault(x => x.PluginId == pluginId);
        if (plugin == null) return false;
        if (plugin.IsEnable)
        {
            if (PluginEntity.Select.Where(x => x.PluginId == pluginId).ToUpdate().Set(x => x.IsEnable, false)
                    .ExecuteAffrows() == 0)
                return false;

            try
            {
                DefaultPlugin.UnloadPlugin(plugin);
                plugin.IsEnable = false;
                NotifyPluginModify(plugin);
            }
            catch (Exception ex)
            {
                HandleHookException(plugin, "UnloadPlugin", ex);
                PluginEntity.Select.Where(x => x.PluginId == pluginId).ToUpdate().Set(x => x.IsEnable, true)
                    .ExecuteAffrows();
                return false;
            }
        }
        else
        {
            if (PluginEntity.Select.Where(x => x.PluginId == pluginId).ToUpdate().Set(x => x.IsEnable, true)
                    .ExecuteAffrows() == 0)
                return false;

            try
            {
                DefaultPlugin.LoadPlugin(plugin);
                plugin.IsEnable = true;
                NotifyPluginModify(plugin);
            }
            catch (Exception ex)
            {
                HandleHookException(plugin, "LoadPlugin", ex);
                PluginEntity.Select.Where(x => x.PluginId == pluginId).ToUpdate().Set(x => x.IsEnable, false)
                    .ExecuteAffrows();
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     删除插件（包括插件文件）
    /// </summary>
    /// <param name="pluginId"></param>
    /// <returns></returns>
    public static bool DeletePlugin(string pluginId)
    {
        var plugin = GetAllPlugins().FirstOrDefault(x => x.PluginId == pluginId);
        if (plugin == null) return false;

        try
        {
            DefaultPlugin.InvokePluginDeleted(plugin);
        }
        catch (Exception ex)
        {
            HandleHookException(plugin, nameof(ISystemPlugin.PluginDeleted), ex);
            // 删除流程不因插件自身清理异常而中断。
        }

        DefaultPlugin.UnloadPlugin(plugin);
        plugin.IsEnable = false;
        NotifyPluginModify(plugin);
        if (plugin.PluginPath.IsNullOrEmpty()) return false;
        Directory.Delete(Path.GetDirectoryName(plugin.PluginPath)!, true);
        return true;
    }

    /// <summary>
    ///     文章页展示时执行
    /// </summary>
    /// <param name="articleModel"></param>
    /// <returns></returns>
    public static ArticleModel OnArticleShow(ArticleModel articleModel)
    {
        foreach (var instance in ArticlePluginCache.GetArticlePlugins())
        {
            ArticleModel ret = null;
            try
            {
                ret = instance?.OnArticleShow(articleModel);
            }
            catch (Exception ex)
            {
                HandleHookException(instance, nameof(IArticlePlugin.OnArticleShow), ex);
                // 单个插件异常不应影响主流程。
            }

            if (ret != null) articleModel = ret;
        }

        return articleModel;
    }

    /// <summary>
    ///     文章列表显示时执行
    /// </summary>
    /// <param name="articlePlugins">文章插件列表</param>
    /// <param name="serviceProvider">服务提供者</param>
    /// <returns></returns>
    public static List<EditorExtModel> OnArticleEditorShow(IEnumerable<IArticlePlugin> articlePlugins,
        IServiceProvider serviceProvider)
    {
        var extModels = new List<EditorExtModel>();
        foreach (var instance in articlePlugins)
        {
            EditorExtModel ret = null;
            try
            {
                ret = instance?.AddEditorToolbarButton(serviceProvider.GetService<DialogService>());
            }
            catch (Exception ex)
            {
                HandleHookException(instance, nameof(IArticlePlugin.AddEditorToolbarButton), ex);
                // 单个插件异常不应影响主流程。
            }

            if (ret != null) extModels.Add(ret);
        }

        return extModels;
    }

    /// <summary>
    ///     内容保存前执行（文章/页面共用）
    /// </summary>
    /// <param name="articlePlugins"></param>
    /// <param name="articleEntity"></param>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool OnContentBeforeSave(IEnumerable<IArticlePlugin> articlePlugins, ArticleEntity articleEntity,
        out string errorMsg)
    {
        errorMsg = "";
        if (articlePlugins == null) return true;

        foreach (var articlePlugin in articlePlugins)
        {
            if (articlePlugin == null) continue;
            try
            {
                if (articlePlugin.OnArticleBeforeSave(articleEntity, out errorMsg)) continue;
                if (errorMsg.IsNullOrEmpty()) errorMsg = $"插件 {articlePlugin.GetType().Name} 拒绝保存。";
                return false;
            }
            catch (Exception ex)
            {
                HandleHookException(articlePlugin, nameof(IArticlePlugin.OnArticleBeforeSave), ex);
                errorMsg = $"插件 {articlePlugin.GetType().Name} 保存前处理失败：{ex.Message}";
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     内容保存后执行（文章/页面共用）
    /// </summary>
    /// <param name="articlePlugins"></param>
    /// <param name="articleEntity"></param>
    public static void OnContentSaved(IEnumerable<IArticlePlugin> articlePlugins, ArticleEntity articleEntity,
        Action<string> onWarning = null)
    {
        if (articlePlugins == null) return;
        foreach (var articlePlugin in articlePlugins)
        {
            try
            {
                articlePlugin?.OnArticleSaved(articleEntity);
            }
            catch (Exception ex)
            {
                HandleHookException(articlePlugin, nameof(IArticlePlugin.OnArticleSaved), ex, onWarning);
                // 单个插件异常不应影响主流程。
            }
        }
    }

    /// <summary>
    ///     获取底部扩展内容
    /// </summary>
    /// <param name="articlePlugins"></param>
    /// <param name="articleEntity"></param>
    /// <returns></returns>
    public static List<ArticleExtModel> GetBottomExtModels(IEnumerable<IArticlePlugin> articlePlugins,
        ArticleEntity articleEntity)
    {
        var articleExtModels = new List<ArticleExtModel>();
        foreach (var articlePlugin in articlePlugins)
        {
            List<ArticleExtModel> ret = null;
            try
            {
                ret = articlePlugin?.BottomExt(articleEntity);
            }
            catch (Exception ex)
            {
                HandleHookException(articlePlugin, nameof(IArticlePlugin.BottomExt), ex);
                // 单个插件异常不应影响主流程。
            }

            if (ret != null) articleExtModels.AddRange(ret);
        }

        return articleExtModels;
    }

    /// <summary>
    ///     获取右侧扩展内容
    /// </summary>
    /// <param name="articlePlugins"></param>
    /// <param name="articleEntity"></param>
    /// <returns></returns>
    public static List<ArticleExtModel> GetRightExtModels(IEnumerable<IArticlePlugin> articlePlugins,
        ArticleEntity articleEntity)
    {
        var articleExtModels = new List<ArticleExtModel>();
        foreach (var articlePlugin in articlePlugins)
        {
            List<ArticleExtModel> ret = null;
            try
            {
                ret = articlePlugin?.RightExt(articleEntity);
            }
            catch (Exception ex)
            {
                HandleHookException(articlePlugin, nameof(IArticlePlugin.RightExt), ex);
                // 单个插件异常不应影响主流程。
            }

            if (ret != null) articleExtModels.AddRange(ret);
        }

        return articleExtModels;
    }

    public static List<PluginMenuModel> OnMenuShow()
    {
        var menuModel = new List<PluginMenuModel>();
        foreach (var instance in SystemPluginCache.GetSystemPlugins())
        {
            List<PluginMenuModel> ret = null;
            try
            {
                ret = instance?.AddMenuItem();
            }
            catch (Exception ex)
            {
                HandleHookException(instance, nameof(ISystemPlugin.AddMenuItem), ex);
                // 单个插件异常不应影响主流程。
            }

            if (ret != null && ret.Count > 0) menuModel.AddRange(ret);
        }

        PluginMenuModels = menuModel;
        return menuModel;
    }

    public static RenderFragment OnPluginPageShow(string menuId)
    {
        if (PluginMenuModels == null || PluginMenuModels.Count == 0) OnMenuShow();
        return PluginMenuModels.FirstOrDefault(x => x.MenuId == menuId)?.PluginBody;
    }
}
