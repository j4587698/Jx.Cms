using Jx.Cms.Common.Extensions;
using Jx.Cms.Plugin.Service.Both;
using Jx.Cms.Rewrite;
using Jx.Toolbox.Extensions;
using Jx.Toolbox.Mvc;

namespace Jx.Cms.Themes.Model;

public class RewriterModel
{
    private static readonly object SyncRoot = new();
    private static RewriterModel _rewriterModel;
    public string RewriteOption { get; set; }

    public string ArticleUrl { get; set; }

    public string PageUrl { get; set; }

    public string IndexUrl { get; set; }

    public string CatalogueUrl { get; set; }

    public string TagUrl { get; set; }

    public string DateUrl { get; set; }

    public static RewriterModel GetSettings()
    {
        if (_rewriterModel != null) return _rewriterModel;

        lock (SyncRoot)
        {
            if (_rewriterModel != null) return _rewriterModel;

            var settingsService = Application.GetService<ISettingsService>();
            if (settingsService == null)
            {
                // 在未安装状态下，返回默认配置
                _rewriterModel = new RewriterModel
                {
                    RewriteOption = nameof(RewriteOptionEnum.Dynamic)
                };
                return _rewriterModel;
            }

            var settingsEnumerable = settingsService.GetAllValues("Rewriter");
            _rewriterModel = new RewriterModel();
            foreach (var settings in settingsEnumerable)
            {
                if (_rewriterModel.GetType().GetProperty(settings.Key) == null) continue;
                _rewriterModel.SetProperty(settings.Key, settings.Value ?? "");
            }
        }

        return _rewriterModel;
    }

    public static void SaveSettings(RewriterModel rewriterModel)
    {
        lock (SyncRoot)
        {
            _rewriterModel = rewriterModel;
        }

        var settingsService = Application.GetService<ISettingsService>();
        if (settingsService == null) return;

        var properties = rewriterModel.GetType().GetProperties();
        foreach (var property in properties)
            settingsService.SetValue("Rewriter", property.Name, property.GetValue(rewriterModel)?.ToString());
    }
}
