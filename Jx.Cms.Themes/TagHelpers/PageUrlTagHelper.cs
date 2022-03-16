using Jx.Cms.Entities.Article;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

[HtmlTargetElement("a", Attributes = "page")]
public class PageUrlTagHelper : TagHelper
{
    public ArticleEntity Page { get; set; }
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        if (Page == null)
        {
            return;
        }
        output.Attributes.SetAttribute("href", RewriteUtil.GetPageUrl(Page));
    }
}