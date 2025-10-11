using Jx.Cms.Plugin.Service.Both;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class TagController : BaseController
{
    public IActionResult Index(int id, int pageNum)
    {
        if (pageNum == 0) pageNum = 1;
        var settings = ViewData["settings"] as SystemSettingsVm;
        if (settings.CountPerPage == 0) settings.CountPerPage = 10;
        var labelService = HttpContext.RequestServices.GetService<ITagService>();
        var label = labelService?.GetTagById(id);
        if (label == null) return NotFound();
        var articles = labelService.GetArticleFromTagId(id, pageNum, settings.CountPerPage, out var totalCount);
        if (articles == null) return NotFound();
        var labelVm = new TagVm
        {
            Articles = articles,
            PageNum = pageNum,
            PageSize = settings.CountPerPage,
            TotalCount = totalCount,
            Tag = label,
            Pagination = HttpContext.RequestServices.GetService<IPaginationService>()
                ?.GetPagination(pageNum, settings.CountPerPage, (int)totalCount)
        };
        return View(labelVm);
    }
}