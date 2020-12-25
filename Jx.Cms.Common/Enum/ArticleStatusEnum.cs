using System.ComponentModel;

namespace Jx.Cms.Common.Enum
{
    /// <summary>
    /// 文章状态
    /// </summary>
    public enum ArticleStatusEnum
    {
        
        [Description("已发布")]
        Published = 1,
        [Description("草稿")]
        Draft,
        [Description("待审核")]
        Reviewed
    }
}