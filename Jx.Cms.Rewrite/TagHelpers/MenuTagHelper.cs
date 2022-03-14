using System;
using Furion;
using Jx.Cms.Common.Enum;
using Jx.Cms.Entities.Front;
using Jx.Cms.Service.Admin;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Rewrite.TagHelpers;

[HtmlTargetElement("a", Attributes = "menu")]
public class MenuTagHelper : TagHelper
{
    public MenuEntity Menu { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        switch (Menu.MenuType)
        {
            case MenuTypeEnum.Page:
                output.Attributes.SetAttribute("href", RewriteUtil.GetPageUrl(App.GetService<IPageService>().GetArticleById(Menu.TypeId)));
                break;
            case MenuTypeEnum.Article:
                output.Attributes.SetAttribute("href", RewriteUtil.GetArticleUrl(App.GetService<IArticleService>().GetArticleById(Menu.TypeId)));
                break;
            case MenuTypeEnum.CustomUrl:
                output.Attributes.SetAttribute("href", Menu.Url);
                break;
            case MenuTypeEnum.Catalogue:
                output.Attributes.SetAttribute("href", RewriteUtil.GetCatalogueUrl(App.GetService<ICatalogService>().FindCatalogById(Menu.TypeId)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (Menu.OpenInNewWindow)
        {
            output.Attributes.SetAttribute("target", "_blank");
        }
        output.Content.SetHtmlContent(Menu.NavTitle);
    }
}