using Jx.Cms.Entities.Article;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Rewrite.TagHelpers;

[HtmlTargetElement("a", Attributes = "article")]
public class ArticleUrlTagHelper : TagHelper
{
    public ArticleEntity Article { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        if (Article == null)
        {
            return;
        }
        output.Attributes.SetAttribute("href", RewriteUtil.GetArticleUrl(Article));
    }
}