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
        var settings = ViewData["settings"] as SystemSettingsVm;
        if (settings.CountPerPage == 0)
        {
            settings.CountPerPage = 10;
        }
        var dateVm = new DateVm
        {
            Articles = App.GetService<ArticleService>()
                .GetArticleWithDate(year, month, pageNum, settings.CountPerPage, out var totalCount),
            Month = month,
            Year = year,
            PageNum = pageNum,
            PageSize = settings.CountPerPage,
            TotalCount = totalCount,
            Pagination = App.GetService<IPaginationService>().GetPagination(pageNum, settings.CountPerPage, totalCount)
        };
        return View(dateVm);
    }
}