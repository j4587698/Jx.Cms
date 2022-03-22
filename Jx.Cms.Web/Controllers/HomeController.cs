using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class HomeController : BaseController
{
    // GET
    public IActionResult Index(int pageNum)
    {
        if (pageNum == 0)
        {
            pageNum = 1;
        }

        var settings = ViewData["settings"] as SystemSettingsVm;
        if (settings.CountPerPage == 0)
        {
            settings.CountPerPage = 10;
        }

        var indexVm = new IndexVm
        {
            Articles = App.GetService<IArticleService>().GetArticlePageWithCount(pageNum, settings.CountPerPage, out var totalCount),
            PageNum = pageNum,
            PageSize = settings.CountPerPage,
            TotalCount = totalCount,
            Pagination = App.GetService<IPaginationService>().GetPagination(pageNum, settings.CountPerPage, (int)totalCount)
        };
        return View(indexVm);
    }
}