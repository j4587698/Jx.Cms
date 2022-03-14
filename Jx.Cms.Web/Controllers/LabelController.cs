using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.Service.Admin;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class LabelController : BaseController
{
    public IActionResult Index(int id, int pageNum)
    {
        if (pageNum == 0)
        {
            pageNum = 1;
        }
        if (!int.TryParse(ViewData[SettingsConstants.CountPerPageKey] as string, out var count))
        {
            count = 10;
        }
        var labelService = App.GetService<ILabelService>();
        var label = labelService.GetLabelById(id);
        if (label == null)
        {
            return new NotFoundResult();
        }
        var articles = labelService.GetArticleFormLabelId(id, pageNum, count, out var totalCount);
        if (articles == null)
        {
            return new NotFoundResult();
        }
        var labelVm = new LabelVm()
        {
            Articles = articles,
            PageNum = pageNum,
            PageSize = count,
            TotalCount = totalCount,
            Label = label
        };
        return View(labelVm);
    }
}