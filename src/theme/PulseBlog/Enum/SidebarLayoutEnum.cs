using System.ComponentModel;

namespace PulseBlog.Enum;

[Description("侧边栏布局")]
public enum SidebarLayoutEnum
{
    [Description("右侧边栏")] Right,
    [Description("左侧边栏")] Left,
    [Description("隐藏侧边栏")] None
}
