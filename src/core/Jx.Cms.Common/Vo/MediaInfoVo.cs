using System;
using System.IO;
using Jx.Cms.Common.Enum;

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
        public string Name { get; set; }

        /// <summary>
        /// 媒体信息
        /// </summary>
        public FileInfo MediaInfo { get; set; }

        /// <summary>
        /// 媒体路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public MediaTypeEnum MediaType { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}