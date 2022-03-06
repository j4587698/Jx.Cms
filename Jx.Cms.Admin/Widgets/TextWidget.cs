using System;
using BootstrapBlazor.Components;
using Jx.Cms.Admin.Components.WidgetCompnents;
using Jx.Cms.Common.Components;
using Jx.Cms.Common.Enum;
using Jx.Cms.Common.Widgets;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Admin.Widgets;

/// <summary>
/// 文本小工具
/// </summary>
public class TextWidget : IWidget
{
    public string Name { get; set; } = "text";

    public string DisplayName { get; set; } = "文本小工具";

    public string Description { get; set; } = "输出任意文本";

    public WidgetComponentBase SystemBody { get; set; } = null;

    public string GetWidgetHtml()
    {
        throw new System.NotImplementedException();
    }
}