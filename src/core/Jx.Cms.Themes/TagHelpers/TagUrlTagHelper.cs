using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

[HtmlTargetElement("a", Attributes = "tag")]
public class TagUrlTagHelper : TagHelper
{
    public TagEntity Tag { get; set; }

    public long TagPageNum { get; set; } = 1;
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        output.Attributes.SetAttribute("href", RewriteUtil.GetTagUrl(Tag, TagPageNum));
    }
}