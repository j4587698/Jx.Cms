using System;
using System.Collections.Generic;
using Jx.Cms.Common.Utils;
using Jx.Cms.Entities.Article;
using Jx.Cms.Service.Both;
using Jx.Cms.Service.Front;
using Masuit.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blogs.Pages.Blogs
{
    public class Index : PageModel
    {

        public int PageNum { get; set; }

        public Dictionary<string, string> Settings { get; set; }

        public List<ArticleEntity> Articles { get; set; }

        public Dictionary<string, int> Pagination { get; set; }

        public long PageCount { get; set; }
        
        public void OnGet([FromQuery]int page, [FromServices]ISettingsService settingsService, [FromServices]IArticleService articleService, [FromServices]IPaginationService paginationService)
        {
            PageNum = page;
            if (page <= 0)
            {
                PageNum = 1;
            }
            Settings = settingsService.GetValuesByNames(new[] {SettingsConstants.TitleKey, 
                SettingsConstants.SubTitleKey, SettingsConstants.UrlKey, SettingsConstants.CopyRightKey});
            Settings.AddOrUpdate(settingsService.GetAllValues("Blogs"));
            Articles = articleService.GetArticlePageWithCount(page, 10, out long count);
            PageCount = count;
            Pagination = paginationService.GetPagination(page, 10, (int)count);
        }
    }
}