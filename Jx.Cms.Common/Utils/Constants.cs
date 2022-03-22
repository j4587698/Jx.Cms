using System.IO;

namespace Jx.Cms.Common.Utils
{
    /// <summary>
    /// 常量表
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 主题的路径
        /// </summary>
        public static readonly string ThemePath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "Theme"));
        
        /// <summary>
        /// 插件的路径
        /// </summary>
        public static readonly string PluginPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "Plugin"));
        
        /// <summary>
        /// 默认菜单名
        /// </summary>
        public const string Menu = "menu";
        
        /// <summary>
        /// 默认友情链接名
        /// </summary>
        public const string Link = "link";

        /// <summary>
        /// 系统默认菜单名
        /// </summary>
        public const string SystemType = "system";
    }
}