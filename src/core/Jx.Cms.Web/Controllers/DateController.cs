using System;
using System.Collections.Generic;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext.Entities.Article;
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
        var articleService = HttpContext.RequestServices.GetService<ArticleService>();
        List<ArticleEntity> articles;
        long totalCount = 0;
        
        if (articleService != null)
        {
            articles = articleService.GetArticleWithDate(year, month, pageNum, settings.CountPerPage, out totalCount);
        }
        else
        {
            articles = new List<ArticleEntity>();
        }
        
        var dateVm = new DateVm
        {
            Articles = articles,
            Month = month,
            Year = year,
            PageNum = pageNum,
            PageSize = settings.CountPerPage,
            TotalCount = totalCount,
            Pagination = HttpContext.RequestServices.GetService<IPaginationService>()?.GetPagination(pageNum, settings.CountPerPage, totalCount)
        };
        return View(dateVm);
    }
}