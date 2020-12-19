using System.ComponentModel;

namespace Jx.Cms.Themes.Config
{
    public enum ThemeType
    {
        /// <summary>
        /// PC主题
        /// </summary>
        [Description("PC主题")]
        PcTheme = 1,
        /// <summary>
        /// 手机主题
        /// </summary>
        [Description("手机主题")]
        MobileTheme,
        /// <summary>
        /// 自适应主题
        /// </summary>
        [Description("自适应主题")]
        AdaptiveTheme
    }
}