using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Rewrite.TagHelpers;

[HtmlTargetElement("a", Attributes = "index-page-num")]
public class IndexUrlTagHelper : TagHelper
{
    public int IndexPageNum { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        output.Attributes.SetAttribute("href", RewriteUtil.GetIndexUrl(IndexPageNum));
    }
}