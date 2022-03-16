using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.Service.Front;
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

        if (!int.TryParse(ViewData[SettingsConstants.CountPerPageKey] as string, out var count))
        {
            count = 10;
        }
        var indexVm = new IndexVm
        {
            Articles = App.GetService<IArticleService>().GetArticlePageWithCount(pageNum, count, out var totalCount),
            PageNum = pageNum,
            PageSize = count,
            TotalCount = totalCount,
            Pagination = App.GetService<IPaginationService>().GetPagination(pageNum, count, (int)totalCount)
        };
        return View(indexVm);
    }
}