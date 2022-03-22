using System;
using System.Threading.Tasks;
using Furion;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Front;
using Jx.Cms.DbContext.Service.Admin;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Jx.Cms.Themes.TagHelpers;

[HtmlTargetElement("a", Attributes = "menu")]
public class MenuUrlTagHelper : TagHelper
{
    public MenuEntity Menu { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await base.ProcessAsync(context, output);
        switch (Menu.MenuType)
        {
            case MenuTypeEnum.Page:
                output.Attributes.SetAttribute("href", RewriteUtil.GetPageUrl(App.GetService<IPageService>().GetPageById(Menu.TypeId)));
                break;
            case MenuTypeEnum.Article:
                output.Attributes.SetAttribute("href", RewriteUtil.GetArticleUrl(App.GetService<IArticleService>().GetArticleById(Menu.TypeId)));
                break;
            case MenuTypeEnum.CustomUrl:
                output.Attributes.SetAttribute("href", Menu.Url);
                break;
            case MenuTypeEnum.Catalogue:
                output.Attributes.SetAttribute("href", RewriteUtil.GetCatalogUrl(App.GetService<ICatalogService>().FindCatalogById(Menu.TypeId)));
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