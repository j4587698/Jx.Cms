using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

[HtmlTargetElement("a", Attributes = "catalog")]
public class CatalogUrlTagHelper : TagHelper
{
    public CatalogEntity Catalog { get; set; }

    public long CatalogPageNum { get; set; } = 1;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        output.Attributes.SetAttribute("href", RewriteUtil.GetCatalogUrl(Catalog, CatalogPageNum));
    }
}