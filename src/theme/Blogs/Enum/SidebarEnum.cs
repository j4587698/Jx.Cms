using System.ComponentModel;

namespace Blogs.Enum
{
    [Description("侧边栏布局")]
    public enum SidebarEnum
    {
        [Description("居右布局")]
        Right,
        [Description("居左布局")]
        Left,
        [Description("无侧边栏布局")]
        None
    }
}