using Jx.Cms.Common.Enum;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Widgets;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

public class SidebarTagHelper : TagHelper
{
    public WidgetSidebarType Sidebar { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        var widgets = WidgetCache.GetSidebarWidgets(Sidebar);
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("id", "sidebar");
        output.Attributes.SetAttribute("class", "widget-area");
        foreach (var widget in widgets)
        {
            TagBuilder widgetTag = new TagBuilder("aside");
            widgetTag.AddCssClass($"widget widget_{widget.Name}");
            var h3Tag = new TagBuilder("h3");
            h3Tag.AddCssClass("widget-title");
            var spanTag = new TagBuilder("span");
            spanTag.AddCssClass("cat");
            spanTag.InnerHtml.Append(widget.GetWidgetName());
            h3Tag.InnerHtml.AppendHtml(spanTag);
            widgetTag.InnerHtml.AppendHtml(h3Tag);
            widgetTag.InnerHtml.AppendHtml(widget.GetWidgetHtml());
            var clearTag = new TagBuilder("div");
            clearTag.AddCssClass("clear");
            widgetTag.InnerHtml.AppendHtml(clearTag);
            output.Content.AppendHtml(widgetTag);
        }
        
    }
}