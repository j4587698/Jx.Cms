using System;
using Furion;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Front;
using Jx.Cms.DbContext.Service.Admin;
using Jx.Cms.DbContext.Service.Both;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

public class MenuTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        output.TagName = null;
        var menus = App.GetService<IMenuService>().GetAllMenuTree();
        if (menus == null || menus.Count == 0)
        {
            output.SuppressOutput();
            return;
        }
        foreach (var menu in menus)
        {
            output.Content.AppendHtml(CreateItem(menu));
        }
    }

    private TagBuilder CreateItem(MenuEntity menuEntity)
    {
        var liTag = new TagBuilder("li");
        liTag.MergeAttribute("class", "navbar-item");
        var aTag = new TagBuilder("a");
        switch (menuEntity.MenuType)
        {
            case MenuTypeEnum.Page:
                aTag.MergeAttribute("href", RewriteUtil.GetPageUrl(App.GetService<IPageService>().GetPageById(menuEntity.TypeId)));
                break;
            case MenuTypeEnum.Article:
                aTag.MergeAttribute("href", RewriteUtil.GetArticleUrl(App.GetService<IArticleService>().GetArticleById(menuEntity.TypeId)));
                break;
            case MenuTypeEnum.CustomUrl:
                aTag.MergeAttribute("href", menuEntity.Url);
                break;
            case MenuTypeEnum.Catalogue:
                aTag.MergeAttribute("href", RewriteUtil.GetCatalogUrl(App.GetService<ICatalogService>().FindCatalogById(menuEntity.TypeId)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (menuEntity.OpenInNewWindow)
        {
            aTag.MergeAttribute("target", "_blank");
        }
        
        aTag.MergeAttribute("title", menuEntity.Title);
        aTag.InnerHtml.AppendHtml(menuEntity.NavTitle);
        liTag.InnerHtml.AppendHtml(aTag);
        if (menuEntity.HasChildren)
        {
            var ulTag = new TagBuilder("ul");
            foreach (var child in menuEntity.Children)
            {
                ulTag.InnerHtml.AppendHtml(CreateItem(child));
            }

            liTag.InnerHtml.AppendHtml(ulTag);
        }

        return liTag;
    }
}