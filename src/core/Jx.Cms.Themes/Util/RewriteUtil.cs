using System.Globalization;
using System.Linq.Expressions;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Rewrite;
using Jx.Cms.Themes.Model;
using Jx.Toolbox.Extensions;
using Jx.Toolbox.Utils;

namespace Jx.Cms.Themes.Util;

public static class RewriteUtil
{
    /// <summary>
    ///     获取文章页伪静态Url
    /// </summary>
    /// <param name="articleEntity">文章信息</param>
    /// <returns></returns>
    public static string GetArticleUrl(ArticleEntity articleEntity)
    {
        var rewriterModel = RewriterModel.GetSettings();
        if (IsDynamic(rewriterModel)) return $"/Post?id={articleEntity.Id}";

        var template = Template.Create(rewriterModel.ArticleUrl);
        return template.SetValue("id", articleEntity.Id.ToString())
            .SetValue("year", articleEntity.PublishTime.Year.ToString())
            .SetValue("month", articleEntity.PublishTime.Month.ToString())
            .SetValue("day", articleEntity.PublishTime.Day.ToString())
            .SetValue("category",
                articleEntity.Catalogue == null ? "" :
                articleEntity.Catalogue.Alias.IsNullOrEmpty() ? articleEntity.Catalogue.Name :
                articleEntity.Catalogue.Alias)
            .SetValue("alias", articleEntity.Alias.IsNullOrEmpty() ? articleEntity.Title : articleEntity.Alias)
            .Render();
    }

    /// <summary>
    ///     获取页面伪静态Url
    /// </summary>
    /// <param name="articleEntity"></param>
    /// <returns></returns>
    public static string GetPageUrl(ArticleEntity articleEntity)
    {
        var rewriterModel = RewriterModel.GetSettings();
        if (IsDynamic(rewriterModel)) return $"/Page?id={articleEntity.Id}";

        var template = Template.Create(rewriterModel.PageUrl);
        return template.SetValue("id", articleEntity.Id.ToString())
            .SetValue("year", articleEntity.PublishTime.Year.ToString())
            .SetValue("month", articleEntity.PublishTime.Month.ToString())
            .SetValue("day", articleEntity.PublishTime.Day.ToString())
            .SetValue("category",
                articleEntity.Catalogue == null ? "" :
                articleEntity.Catalogue.Alias.IsNullOrEmpty() ? articleEntity.Catalogue.Name :
                articleEntity.Catalogue.Alias)
            .SetValue("alias", articleEntity.Alias.IsNullOrEmpty() ? articleEntity.Title : articleEntity.Alias)
            .Render();
    }

    /// <summary>
    ///     获取首页Url
    /// </summary>
    /// <param name="pageNo">当前为第几页</param>
    /// <returns></returns>
    public static string GetIndexUrl(long pageNo = 1)
    {
        var rewriterModel = RewriterModel.GetSettings();
        if (IsDynamic(rewriterModel)) return $"/?pageNum={pageNo}";

        var template = Template.Create(rewriterModel.IndexUrl);
        return template.SetValue("page", pageNo.ToString()).Render();
    }

    /// <summary>
    ///     获取标签页Url
    /// </summary>
    /// <param name="tag">标签</param>
    /// <param name="pageNo">第几页</param>
    /// <returns></returns>
    public static string GetTagUrl(TagEntity tag, long pageNo = 1)
    {
        var rewriterModel = RewriterModel.GetSettings();
        if (IsDynamic(rewriterModel)) return $"/Tag?id={tag.Id}&pageNum={pageNo}";

        var template = Template.Create(rewriterModel.TagUrl);
        return template.SetValue("id", tag.Id.ToString()).SetValue("page", pageNo.ToString())
            .SetValue("alias", tag.Name).Render();
    }

    /// <summary>
    ///     获取分类标签
    /// </summary>
    /// <param name="catalogEntity"></param>
    /// <param name="pageNo"></param>
    /// <returns></returns>
    public static string GetCatalogUrl(CatalogEntity catalogEntity, long pageNo = 1)
    {
        var rewriterModel = RewriterModel.GetSettings();
        if (IsDynamic(rewriterModel)) return $"/Catalog?id={catalogEntity.Id}&pageNum={pageNo}";

        var template = Template.Create(rewriterModel.CatalogueUrl);
        return template.SetValue("id", catalogEntity.Id.ToString()).SetValue("page", pageNo.ToString())
            .SetValue("alias", catalogEntity.Alias.IsNullOrEmpty() ? catalogEntity.Name : catalogEntity.Alias).Render();
    }

    public static string GetDateUrl(int year, int month, long pageNo)
    {
        var rewriterModel = RewriterModel.GetSettings();
        if (IsDynamic(rewriterModel)) return $"/Date?year={year}&month={month}&pageNum={pageNo}";

        var template = Template.Create(rewriterModel.DateUrl);
        return template.SetValue("year", year.ToString()).SetValue("month", month.ToString())
            .SetValue("page", pageNo.ToString()).Render();
    }

    public static string AnalysisArticle(string url, RewriterModel rewriterModel)
    {
        var values = MatchUrl(url, rewriterModel?.ArticleUrl);
        if (values == null) return null;

        if (values.TryGetValue("id", out var id)) return $"?id={id}";

        Expression<Func<ArticleEntity, bool>> where = x => x.IsPage == false;
        foreach (var info in values)
        {
            switch (info.Key)
            {
                case "year":
                    if (!TryParseInt(info.Value, out var year)) return null;
                    where = where.And(x => x.PublishTime.Year == year);
                    break;
                case "month":
                    if (!TryParseInt(info.Value, out var month)) return null;
                    where = where.And(x => x.PublishTime.Month == month);
                    break;
                case "day":
                    if (!TryParseInt(info.Value, out var day)) return null;
                    where = where.And(x => x.PublishTime.Day == day);
                    break;
                case "alias":
                    var articleAlias = info.Value;
                    where = where.And(x => x.Alias == articleAlias || x.Title == articleAlias);
                    break;
                case "category":
                    var category = info.Value;
                    where = where.And(x => x.Catalogue.Alias == category || x.Catalogue.Name == category);
                    break;
            }
        }

        var article = ArticleEntity.Select.Where(where).First();
        return article == null ? null : $"?id={article.Id}";
    }

    public static string AnalysisPage(string url, RewriterModel rewriterModel)
    {
        var values = MatchUrl(url, rewriterModel?.PageUrl);
        if (values == null) return null;

        if (values.TryGetValue("id", out var id)) return $"?id={id}";

        Expression<Func<ArticleEntity, bool>> where = x => x.IsPage == true;
        foreach (var info in values)
        {
            switch (info.Key)
            {
                case "year":
                    if (!TryParseInt(info.Value, out var year)) return null;
                    where = where.And(x => x.PublishTime.Year == year);
                    break;
                case "month":
                    if (!TryParseInt(info.Value, out var month)) return null;
                    where = where.And(x => x.PublishTime.Month == month);
                    break;
                case "day":
                    if (!TryParseInt(info.Value, out var day)) return null;
                    where = where.And(x => x.PublishTime.Day == day);
                    break;
                case "alias":
                    var pageAlias = info.Value;
                    where = where.And(x => x.Alias == pageAlias || x.Title == pageAlias);
                    break;
                case "category":
                    var category = info.Value;
                    where = where.And(x => x.Catalogue.Alias == category || x.Catalogue.Name == category);
                    break;
            }
        }

        var page = ArticleEntity.Select.Where(where).First();
        return page == null ? null : $"?id={page.Id}";
    }

    public static string AnalysisIndex(string url, RewriterModel rewriterModel)
    {
        var values = MatchUrl(url, rewriterModel?.IndexUrl);
        if (values == null) return null;
        return values.TryGetValue("page", out var page) ? $"?pageNum={page}" : null;
    }

    public static string AnalysisTag(string url, RewriterModel rewriterModel)
    {
        var values = MatchUrl(url, rewriterModel?.TagUrl);
        if (values == null || !values.TryGetValue("page", out var page)) return null;

        if (values.TryGetValue("id", out var id)) return $"?id={id}&pageNum={page}";

        if (values.TryGetValue("alias", out var alias))
        {
            var labelEntity = TagEntity.Where(x => x.Name == alias).First();
            return labelEntity == null ? null : $"?id={labelEntity.Id}&pageNum={page}";
        }

        return null;
    }

    public static string AnalysisCatalog(string url, RewriterModel rewriterModel)
    {
        var values = MatchUrl(url, rewriterModel?.CatalogueUrl);
        if (values == null || !values.TryGetValue("page", out var page)) return null;

        if (values.TryGetValue("id", out var id)) return $"?id={id}&pageNum={page}";

        if (values.TryGetValue("alias", out var alias))
        {
            var catalogEntity = CatalogEntity.Where(x => x.Alias == alias || x.Name == alias).First();
            return catalogEntity == null ? null : $"?id={catalogEntity.Id}&pageNum={page}";
        }

        return null;
    }

    public static string AnalysisDate(string url, RewriterModel rewriterModel)
    {
        var values = MatchUrl(url, rewriterModel?.DateUrl);
        if (values == null) return null;

        if (!(values.TryGetValue("page", out var page) && values.TryGetValue("year", out var year) &&
              values.TryGetValue("month", out var month))) return null;

        if (!(TryParseLong(page, out _) && TryParseInt(year, out _) && TryParseInt(month, out _))) return null;
        return $"?year={year}&month={month}&pageNum={page}";
    }

    private static Dictionary<string, string> MatchUrl(string url, string template)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(template)) return null;

        var templateList = RewriteTemplate.CreateUrl(template);
        if (templateList.Count == 0) return null;

        var result = RewriteTemplate.AnalysisUrl(url, templateList);
        return result.isSuccess ? result.result : null;
    }

    private static bool TryParseInt(string value, out int result)
    {
        return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result);
    }

    private static bool TryParseLong(string value, out long result)
    {
        return long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result);
    }

    private static bool IsDynamic(RewriterModel rewriterModel)
    {
        return rewriterModel == null || rewriterModel.RewriteOption.IsNullOrEmpty() ||
               string.Equals(rewriterModel.RewriteOption, nameof(RewriteOptionEnum.Dynamic),
                   StringComparison.OrdinalIgnoreCase);
    }
}
