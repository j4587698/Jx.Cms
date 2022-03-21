using Jx.Cms.Entities.Article;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

[HtmlTargetElement("a", Attributes = "catalogue")]
public class CatalogueUrlTagHelper : TagHelper
{
    public CatalogEntity Catalogue { get; set; }

    public long CataloguePageNum { get; set; } = 1;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        output.Attributes.SetAttribute("href", RewriteUtil.GetCatalogueUrl(Catalogue, CataloguePageNum));
    }
}