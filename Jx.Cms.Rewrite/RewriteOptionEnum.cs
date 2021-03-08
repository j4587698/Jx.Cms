using System.ComponentModel;

namespace Jx.Cms.Rewrite
{
    /// <summary>
    /// 伪静态Enum
    /// </summary>
    public enum RewriteOptionEnum
    {
        /// <summary>
        /// 动态
        /// </summary>
        [Description("动态")]
        Dynamic,
        /// <summary>
        /// 伪静态
        /// </summary>
        [Description("伪静态")]
        Rewrite
    }
}