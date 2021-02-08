using System.ComponentModel.DataAnnotations;
using Jx.Cms.Common.Utils;
using Jx.Cms.Service.Both;

namespace Jx.Cms.Admin.ViewModel
{
    public class SystemSettingsVm
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        [Required]
        public string SubTitle { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        [Required]
        public string Url { get; set; }

        /// <summary>
        /// 版权信息
        /// </summary>
        [Required]
        public string CopyRight { get; set; }

        public static SystemSettingsVm Init()
        {
            var settings = new SystemSettingsVm();
            var settingsService = Furion.App.GetService<ISettingsService>();
            settings.Title = settingsService.GetValue(SettingsConstants.TitleKey) ?? "JXCMS";
            settings.SubTitle = settingsService.GetValue(SettingsConstants.SubTitleKey) ?? "a new open source asp.net cms";
            settings.CopyRight = settingsService.GetValue(SettingsConstants.CopyRightKey) ?? "Copyright Your WebSite.Some Rights Reserved.";
            settings.Url = settingsService.GetValue(SettingsConstants.UrlKey);
            if (settings.Url == null)
            {
                var request = Furion.App.HttpContext.Request;
                settings.Url = $"{request.Scheme}://{request.Host}";
            }
            return settings;
        }

        public void Save()
        {
            var settingsService = Furion.App.GetService<ISettingsService>();
            settingsService.SetValue(SettingsConstants.TitleKey, Title);
            settingsService.SetValue(SettingsConstants.SubTitleKey, SubTitle);
            settingsService.SetValue(SettingsConstants.CopyRightKey, CopyRight);
            settingsService.SetValue(SettingsConstants.UrlKey, Url);
        }
    }
}