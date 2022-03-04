using BootstrapBlazor.Components;
using Jx.Cms.Common.Enum;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Common.Components;

/// <summary>
/// 小工具基类
/// </summary>
public class WidgetComponentBase: BootstrapComponentBase
{
    /// <summary>
    /// 小工具所在菜单
    /// </summary>
    [Parameter]
    public WidgetMenuType WidgetMenuType { get; set; }

    /// <summary>
    /// 点击保存按钮时调用
    /// <returns>是否保存成功</returns>
    /// </summary>
    public virtual bool OnSave()
    {
        return true;
    }
    
}