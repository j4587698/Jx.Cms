namespace Jx.Cms.Themes
{
    /// <summary>
    /// 切换主题方式
    /// </summary>
    public enum ThemeChangeMode
    {
        /// <summary>
        /// 不切换
        /// </summary>
        None,
        /// <summary>
        /// 自适应
        /// </summary>
        Adaptive,
        /// <summary>
        /// 自动切换主题
        /// </summary>
        Auto,
        /// <summary>
        /// 根据域名切换主题
        /// </summary>
        Domain
    }
}