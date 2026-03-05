using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable All

namespace Jx.Cms.Web.Controllers;

public class CatalogController : BaseController
{
    private readonly ICatalogService _catalogService;
    private readonly IPaginationService _paginationService;

    public CatalogController(ICatalogService catalogService, IPaginationService paginationService)
    {
        _catalogService = catalogService;
        _paginationService = paginationService;
    }

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

        var catalogue = _catalogService?.GetCatalogById(id);
        if (catalogue == null)
        {
            return NotFound();
        }

        var catalogVm = new CatalogVm();
        catalogVm.Articles =
            _catalogService.GetArticlesByCatalogId(id, true, pageNum, settings.CountPerPage, out var totalPage);
        catalogVm.Catalog = catalogue;
        catalogVm.PageNum = pageNum;
        catalogVm.PageSize = settings.CountPerPage;
        catalogVm.TotalCount = totalPage;
        catalogVm.Pagination = _paginationService?.GetPagination(pageNum, settings.CountPerPage, (int)totalPage);
        return View(catalogVm);
    }
}
