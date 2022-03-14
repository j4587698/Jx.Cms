using Jx.Cms.Entities.Article;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Rewrite.TagHelpers;

[HtmlTargetElement("a", Attributes = "catalogue")]
public class CatalogueUrlTagHelper : TagHelper
{
    public CatalogEntity Catalogue { get; set; }

    public int CataloguePageNum { get; set; } = 1;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        output.Attributes.SetAttribute("href", RewriteUtil.GetCatalogueUrl(Catalogue, CataloguePageNum));
    }
}