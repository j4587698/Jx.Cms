﻿namespace Jx.Cms.Common.Utils
{
    public class ThemeConfig
    {
        /// <summary>
        /// 主题名称
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// 主题显示名称
        /// </summary>
        public string ThemeDisplayName { get; set; }

        /// <summary>
        /// 主题描述
        /// </summary>
        public string ThemeDescription { get; set; }

        /// <summary>
        /// 主题版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 主题路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 主题截图名称
        /// </summary>
        public string ScreenShot { get; set; }

        /// <summary>
        /// 主题类型
        /// </summary>
        public ThemeType ThemeType { get; set; }

        /// <summary>
        /// 是否正在使用
        /// </summary>
        public bool IsUsing { get; set; }
    }
}