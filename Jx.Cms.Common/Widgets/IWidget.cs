using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Common.Widgets;

/// <summary>
/// 小工具类
/// </summary>
public interface IWidget
{
    /// <summary>
    /// 小工具名称
    /// </summary>
    string Name { get; set; }
    
    /// <summary>
    /// 显示名称
    /// </summary>
    string DisplayName { get; set; }
    
    /// <summary>
    /// 小工具描述
    /// </summary>
    string Description { get; set; }
    
    /// <summary>
    /// 后台显示的内容
    /// </summary>
    RenderFragment SystemBody { get; set; }

    /// <summary>
    /// 获取小工具的Html代码
    /// </summary>
    /// <returns></returns>
    string GetWidgetHtml();
}