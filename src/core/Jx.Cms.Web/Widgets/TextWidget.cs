using System;
using System.Collections.Generic;
using Furion.JsonSerialization;
using Jx.Cms.Plugin.Plugin;
using Jx.Cms.Web.Components.WidgetCompnents;
using Masuit.Tools;

namespace Jx.Cms.Web.Widgets;

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
        if (Parameter.IsNullOrEmpty())
        {
            return "";
        }
        var parameters = JSON.Deserialize<Dictionary<string, string>>(Parameter);
        return parameters.ContainsKey("Title") ? parameters["Title"] : "";
    }

    public string GetWidgetHtml()
    {
        if (Parameter.IsNullOrEmpty())
        {
            return "";
        }
        var parameters = JSON.Deserialize<Dictionary<string, string>>(Parameter);
        return $"<div class=\"textwidget\">{(parameters.ContainsKey("Content") ? parameters["Content"] : "")}</div>";
    }
}