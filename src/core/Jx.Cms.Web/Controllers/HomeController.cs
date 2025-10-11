using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class HomeController : BaseController
{
    // GET
    public IActionResult Index(int pageNum)
    {
        if (pageNum == 0) pageNum = 1;

        var settings = ViewData["settings"] as SystemSettingsVm;
        if (settings.CountPerPage == 0) settings.CountPerPage = 10;

        var articleService = HttpContext.RequestServices.GetService<IArticleService>();
        List<ArticleEntity> articles;
        long totalCount = 0;

        if (articleService != null)
            articles = articleService.GetArticlePageWithCount(pageNum, settings.CountPerPage, out totalCount);
        else
            articles = new List<ArticleEntity>();

        var indexVm = new IndexVm
        {
            Articles = articles,
            PageNum = pageNum,
            PageSize = settings.CountPerPage,
            TotalCount = totalCount,
            Pagination = HttpContext.RequestServices.GetService<IPaginationService>()
                ?.GetPagination(pageNum, settings.CountPerPage, (int)totalCount)
        };
        return View(indexVm);
    }
}