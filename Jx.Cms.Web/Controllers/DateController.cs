using System;
using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Plugin.Service.Front.Impl;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class DateController : BaseController
{
    // GET
    public IActionResult Index(int year, int month, int pageNum)
    {
        if (!int.TryParse(ViewData[SettingsConstants.CountPerPageKey] as string, out var count))
        {
            count = 10;
        }
        var dateVm = new DateVm
        {
            Articles = App.GetService<ArticleService>()
                .GetArticleWithDate(year, month, pageNum, count, out var totalCount),
            Month = month,
            Year = year,
            PageNum = pageNum,
            PageSize = count,
            TotalCount = totalCount,
            Pagination = App.GetService<IPaginationService>().GetPagination(pageNum, count, totalCount)
        };
        return View(dateVm);
    }
}