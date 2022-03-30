using System.ComponentModel;

namespace Blogs.Enum
{
    [Description("布局设置")]
    public enum BlogLayoutEnum
    {
        [Description("居左布局")]
        Left,
        [Description("居右布局")]
        Right,
        [Description("居上布局")]
        Top,
        [Description("居下布局")]
        Bottom
    }
}