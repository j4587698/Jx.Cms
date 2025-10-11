using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable All

namespace Jx.Cms.Web.Controllers;

public class CatalogController : BaseController
{
    // GET
    public IActionResult Index(int id, int pageNum)
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

        var catalogueService = HttpContext.RequestServices.GetService<ICatalogService>();
        var catalogue = catalogueService?.GetCatalogById(id);
        if (catalogue == null)
        {
            return NotFound();
        }

        var catalogVm = new CatalogVm();
        catalogVm.Articles =
            catalogueService.GetArticlesByCatalogId(id, false, pageNum, settings.CountPerPage, out var totalPage);
        catalogVm.Catalog = catalogue;
        catalogVm.PageNum = pageNum;
        catalogVm.PageSize = settings.CountPerPage;
        catalogVm.TotalCount = totalPage;
        catalogVm.Pagination = HttpContext.RequestServices.GetService<IPaginationService>()
            ?.GetPagination(pageNum, settings.CountPerPage, (int)totalPage);
        return View(catalogVm);
    }
}