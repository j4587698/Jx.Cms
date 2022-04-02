namespace Jx.Cms.Common.Utils
{
    /// <summary>
    /// 插件设置类
    /// </summary>
    public class PluginConfig
    {
        /// <summary>
        /// 插件Id，唯一
        /// </summary>
        public string PluginId { get; set; }

        /// <summary>
        /// 插件名
        /// </summary>
        public string PluginName { get; set; }

        /// <summary>
        /// 插件描述
        /// </summary>
        public string PluginDescription { get; set; }

        /// <summary>
        /// 插件版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 插件路径
        /// </summary>
        public string PluginPath { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
    }
}