using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

[HtmlTargetElement("a", Attributes = "year, month")]
public class DateUrlTagHelper : TagHelper
{
    public int Year { get; set; }

    public int Month { get; set; }
    
    public long DatePageNum { get; set; } = 1;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        output.Attributes.SetAttribute("href", RewriteUtil.GetDateUrl(Year, Month, DatePageNum));
    }
}