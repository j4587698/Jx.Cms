using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class HomeController : BaseController
{
    private readonly IArticleService _articleService;
    private readonly IPaginationService _paginationService;

    public HomeController(IArticleService articleService, IPaginationService paginationService)
    {
        _articleService = articleService;
        _paginationService = paginationService;
    }

    // GET
    public IActionResult Index(int pageNum)
    {
        if (pageNum == 0) pageNum = 1;

        var settings = ViewData["settings"] as SystemSettingsVm;
        if (settings.CountPerPage == 0) settings.CountPerPage = 10;

        List<ArticleEntity> articles;
        long totalCount = 0;

        if (_articleService != null)
            articles = _articleService.GetArticlePageWithCount(pageNum, settings.CountPerPage, out totalCount);
        else
            articles = new List<ArticleEntity>();

        var indexVm = new IndexVm
        {
            Articles = articles,
            PageNum = pageNum,
            PageSize = settings.CountPerPage,
            TotalCount = totalCount,
            Pagination = _paginationService?.GetPagination(pageNum, settings.CountPerPage, (int)totalCount)
        };
        return View(indexVm);
    }
}