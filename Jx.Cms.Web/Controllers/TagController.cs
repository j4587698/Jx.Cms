using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext.Service.Both;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class TagController : BaseController
{
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
        var labelService = App.GetService<ITagService>();
        var label = labelService.GetLabelById(id);
        if (label == null)
        {
            return new NotFoundResult();
        }
        var articles = labelService.GetArticleFormLabelId(id, pageNum, settings.CountPerPage, out var totalCount);
        if (articles == null)
        {
            return new NotFoundResult();
        }
        var labelVm = new LabelVm()
        {
            Articles = articles,
            PageNum = pageNum,
            PageSize = settings.CountPerPage,
            TotalCount = totalCount,
            Tag = label,
            Pagination = App.GetService<IPaginationService>().GetPagination(pageNum, settings.CountPerPage, (int)totalCount)
        };
        return View(labelVm);
    }
}