using System.IO;

namespace Jx.Cms.Common.Vo
{
    /// <summary>
    /// 媒体文件信息类
    /// </summary>
    public class MediaInfoVo
    {
        /// <summary>
        /// 媒体名称
        /// </summary>
        public string MediaName { get; set; }

        /// <summary>
        /// 媒体信息
        /// </summary>
        public FileInfo MediaInfo { get; set; }

        /// <summary>
        /// 媒体路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 内容，仅供后台使用
        /// </summary>
        public string Content { get; set; }
    }
}