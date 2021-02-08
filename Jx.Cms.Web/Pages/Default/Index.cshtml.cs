using System;
using System.IO;
using System.Reflection;
using Furion;
using Jx.Cms.Service.Front;
using Jx.Cms.Themes;
using Jx.Cms.Themes.Options;
using Jx.Cms.Themes.PartManager;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Jx.Cms.Web.Pages.Default
{
    public class Index : DefaultPageModel
    {
        public void OnGet(int pageNo)
        {
            if (pageNo == 0)
            {
                pageNo = 1;
            }
            var articleService = App.GetService<IArticleService>();
            ViewData["articles"] = articleService.GetArticlePage(pageNo, 10);
        }
    }
}