using System;
using System.ComponentModel.DataAnnotations;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.Service.Both;
using Jx.Toolbox.Extensions;

namespace Jx.Cms.Themes.Vm
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

        /// <summary>
        /// 备案信息
        /// </summary>
        public string BeiAn { get; set; }

        /// <summary>
        /// 每页显示数量
        /// </summary>
        [Required]
        public int CountPerPage { get; set; }
        
        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool CanComment { get; set; }
        
        /// <summary>
        /// 评论需要被审核
        /// </summary>
        public bool CommentNeedVerify { get; set; }

        /// <summary>
        /// 文章编辑时默认使用Markdown编辑器
        /// </summary>
        public bool EditWithMarkdown { get; set; }

        public static SystemSettingsVm Init()
        {
            var settings = new SystemSettingsVm();
            var settingsService = Furion.App.GetService<ISettingsService>();
            var values = settingsService.GetAllValues();
            var properties = settings.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (values.ContainsKey(property.Name))
                {
                    property.SetValue(settings,
                        property.PropertyType != typeof(string)
                            ? Convert.ChangeType(values[property.Name], property.PropertyType)
                            : values[property.Name]);
                }
            }

            if (!settings.Url.IsNullOrEmpty()) return settings;
            var request = Furion.App.HttpContext.Request;
            settings.Url = $"{request.Scheme}://{request.Host}";
            return settings;
        }

        public void Save()
        {
            var settingsService = Furion.App.GetService<ISettingsService>();
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                settingsService.SetValue(property.Name, property.GetValue(this)?.ToString());
            }
        }
    }
}