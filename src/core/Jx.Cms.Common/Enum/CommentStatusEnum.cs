using System.ComponentModel;

namespace Jx.Cms.Common.Enum
{
    [Description("评论状态")]
    public enum CommentStatusEnum
    {
        [Description("已通过")]
        Pass,
        [Description("待审核")]
        NeedCheck,
        
    }
}