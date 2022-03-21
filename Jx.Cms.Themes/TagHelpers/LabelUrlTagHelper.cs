using Jx.Cms.Entities.Article;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

[HtmlTargetElement("a", Attributes = "label")]
public class LabelUrlTagHelper : TagHelper
{
    public LabelEntity Label { get; set; }

    public long LabelPageNum { get; set; } = 1;
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        output.Attributes.SetAttribute("href", RewriteUtil.GetLabelUrl(Label, LabelPageNum));
    }
}