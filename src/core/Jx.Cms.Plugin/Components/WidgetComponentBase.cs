using System;
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
    /// 小工具的参数
    /// </summary>
    [Parameter]
    public string Parameter { get; set; }

    /// <summary>
    /// 小工具的唯一ID
    /// </summary>
    [Parameter]
    public Guid WidgetId { get; set; }
    
    /// <summary>
    /// 当需要保存参数时调用
    /// </summary>
    [Parameter]
    public EventCallback<string> OnParameterSave { get; set; }

}