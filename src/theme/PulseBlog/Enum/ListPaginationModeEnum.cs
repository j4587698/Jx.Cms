using System.ComponentModel;

namespace PulseBlog.Enum;

[Description("列表分页模式")]
public enum ListPaginationModeEnum
{
    [Description("传统分页")] Pager,
    [Description("点击加载更多")] LoadMore,
    [Description("下拉自动加载")] AutoLoad
}
