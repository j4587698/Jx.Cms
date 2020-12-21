using System.Collections.Generic;
using System.IO;
using Jx.Cms.Common.Utils;

namespace Jx.Cms.Service
{
    /// <summary>
    /// 主题相关
    /// </summary>
    public interface IThemeConfigService
    {
        /// <summary>
        /// 获取所有主题
        /// </summary>
        /// <returns>主题信息列表</returns>
        List<ThemeConfig> GetAllThemes();

        /// <summary>
        /// 获取主题对应的主题图片
        /// </summary>
        /// <param name="themeName">主题名</param>
        /// <returns>主题图片流</returns>
        Stream GetScreenShotStreamByThemeName(string themeName);

        /// <summary>
        /// 启用指定主题
        /// </summary>
        /// <param name="themeConfig">主题信息</param>
        /// <returns>是否成功</returns>
        bool EnableTheme(ThemeConfig themeConfig);
    }
}