using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;
// ReSharper disable All

namespace Jx.Cms.Web.Controllers;

public class CatalogueController : BaseController
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

        var catalogueService = App.GetService<ICatalogService>();
        var catalogue = catalogueService.GetCatalogueById(id);
        if (catalogue == null)
        {
            return new NotFoundResult();
        }

        var catalogueVm = new CatalogueVm();
        catalogueVm.Articles = catalogueService.GetArticlesByCatalogueId(id, false, pageNum, settings.CountPerPage, out var totalPage);
        catalogueVm.Catalogue = catalogue;
        catalogueVm.PageNum = pageNum;
        catalogueVm.PageSize = settings.CountPerPage;
        catalogueVm.TotalCount = totalPage;
        catalogueVm.Pagination = App.GetService<IPaginationService>().GetPagination(pageNum, settings.CountPerPage, (int)totalPage);
        return View(catalogueVm);
    }
}