using System.Text.RegularExpressions;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Jx.Cms.Web.Controllers;

[EnableRateLimiting("search")]
public partial class SearchController : BaseController
{
    private const int DefaultPageNum = 1;
    private const int MaxKeywordLength = 64;
    private readonly IArticleService _articleService;
    private readonly IPaginationService _paginationService;

    public SearchController(IArticleService articleService, IPaginationService paginationService)
    {
        _articleService = articleService;
        _paginationService = paginationService;
    }

    public IActionResult Index(string q, int pageNum)
    {
        if (pageNum <= 0) pageNum = DefaultPageNum;

        var settings = ViewData["settings"] as SystemSettingsVm;
        if (settings is { CountPerPage: 0 }) settings.CountPerPage = 10;

        var keyword = NormalizeKeyword(q);
        List<ArticleEntity> articles;
        long totalCount;
        if (keyword.Length == 0 || _articleService == null)
        {
            articles = new List<ArticleEntity>();
            totalCount = 0;
        }
        else
        {
            articles = _articleService.SearchArticles(keyword, pageNum, settings?.CountPerPage ?? 10, out totalCount);
        }

        var searchVm = new SearchVm
        {
            Query = keyword,
            Articles = articles,
            PageNum = pageNum,
            PageSize = settings?.CountPerPage ?? 10,
            TotalCount = totalCount,
            Pagination = keyword.Length == 0
                ? new Dictionary<string, long>()
                : _paginationService?.GetPagination(pageNum, settings?.CountPerPage ?? 10, totalCount)
        };

        return View(searchVm);
    }

    private static string NormalizeKeyword(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return string.Empty;
        var normalized = HtmlTagRegex().Replace(keyword, " ");
        normalized = MultiSpaceRegex().Replace(normalized, " ").Trim();
        if (normalized.Length > MaxKeywordLength)
            normalized = normalized[..MaxKeywordLength];
        return normalized;
    }

    [GeneratedRegex("<.*?>", RegexOptions.Singleline)]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex("\\s+", RegexOptions.Singleline)]
    private static partial Regex MultiSpaceRegex();
}
