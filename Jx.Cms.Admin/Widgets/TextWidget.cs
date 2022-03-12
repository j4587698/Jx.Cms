using System;
using System.Collections.Generic;
using BootstrapBlazor.Components;
using Furion.JsonSerialization;
using Jx.Cms.Admin.Components.WidgetCompnents;
using Jx.Cms.Common.Components;
using Jx.Cms.Common.Enum;
using Jx.Cms.Plugin.Widgets;
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

    public Type SystemBodyType { get; set; } = typeof(TextCompnent);
    
    public string Parameter { get; set; }

    public string GetWidgetName()
    {
        var parameters = JSON.Deserialize<Dictionary<string, string>>(Parameter);
        return parameters.ContainsKey("Title") ? parameters["Title"] : "";
    }

    public string GetWidgetHtml()
    {
        var parameters = JSON.Deserialize<Dictionary<string, string>>(Parameter);
        return $"<div class=\"textwidget\">{(parameters.ContainsKey("Content") ? parameters["Content"] : "")}</div>";
    }
}