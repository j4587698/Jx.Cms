using Jx.Cms.Plugin.Service.Both;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class TagController : BaseController
{
    private readonly ITagService _tagService;
    private readonly IPaginationService _paginationService;

    public TagController(ITagService tagService, IPaginationService paginationService)
    {
        _tagService = tagService;
        _paginationService = paginationService;
    }

    public IActionResult Index(int id, int pageNum)
    {
        if (pageNum == 0) pageNum = 1;
        var settings = ViewData["settings"] as SystemSettingsVm;
        if (settings.CountPerPage == 0) settings.CountPerPage = 10;
        var label = _tagService?.GetTagById(id);
        if (label == null) return NotFound();
        var articles = _tagService.GetArticleFromTagId(id, pageNum, settings.CountPerPage, out var totalCount);
        if (articles == null) return NotFound();
        var labelVm = new TagVm
        {
            Articles = articles,
            PageNum = pageNum,
            PageSize = settings.CountPerPage,
            TotalCount = totalCount,
            Tag = label,
            Pagination = _paginationService?.GetPagination(pageNum, settings.CountPerPage, (int)totalCount)
        };
        return View(labelVm);
    }
}