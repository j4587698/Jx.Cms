using Furion;
using Jx.Cms.DbContext.Service.Both;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

public class FriendLinkTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        var links = App.GetService<IMenuService>().GetAllLinks();
        if (links == null || links.Count == 0)
        {
            output.SuppressOutput();
            return;
        }
        output.TagName = null;
        foreach (var menuEntity in links)
        {
            var liTag = new TagBuilder("li");
            var aTag = new TagBuilder("a");
            aTag.MergeAttribute("href", menuEntity.Url);
            if (menuEntity.OpenInNewWindow)
            {
                aTag.MergeAttribute("target", "_blank");
            }
            aTag.MergeAttribute("title", menuEntity.Title);
            aTag.InnerHtml.AppendHtml(menuEntity.NavTitle);
            liTag.InnerHtml.AppendHtml(aTag);
            output.Content.AppendHtml(liTag);
        }
    }
}