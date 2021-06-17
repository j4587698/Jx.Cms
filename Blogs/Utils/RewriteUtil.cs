using System.Collections.Generic;
using System.Linq;
using Blogs.Model;
using Jx.Cms.Entities.Article;
using Jx.Cms.Rewrite;
using Masuit.Tools;
using Masuit.Tools.Strings;
using Microsoft.AspNetCore.Rewrite;

namespace Blogs.Utils
{
    public static class RewriteUtil
    {
        private static List<string> _articleList;
        public static string GetArticleUrl(ArticleEntity articleEntity)
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return $"/Article?id={articleEntity.Id}";
            }

            var template = Template.Create(rewriterModel.ArticleUrl);
            return template.Set("id", articleEntity.Id.ToString()).Set("year", articleEntity.PublishTime.Year.ToString())
                .Set("month", articleEntity.PublishTime.Month.ToString())
                .Set("day", articleEntity.PublishTime.Day.ToString())
                .Set("alias", articleEntity.Alias.IsNullOrEmpty() ? articleEntity.Title : articleEntity.Alias).Render();
        }

        public static string AnalysisArticle(string url)
        {
            var rewriterModel = RewriterModel.GetSettings();
            _articleList ??= RewriteTemplate.CreateUrl(rewriterModel.ArticleUrl);

            if (_articleList == null || _articleList.Count == 0)
            {
                _articleList = null;
                return null;
            }

            var result = RewriteTemplate.AnalysisUrl(url, _articleList);
            if (result.isSuccess)
            {
                if (result.result.ContainsKey("id"))
                {
                    return $"?id={result.result["id"]}";
                }
                if (result.result.ContainsKey("alias"))
                {
                    var articles = ArticleEntity.Select.Where(x =>
                        x.Alias == result.result["alias"] || x.Title == result.result["alias"]).ToList();
                    if (articles == null)
                    {
                        return null;
                    }
                    if (articles.Count == 0)
                    {
                        return $"?id={articles[0].Id}";
                    }

                    if (articles.Any(x => x.Alias == result.result["alias"]))
                    {
                        return $"?id={articles.First(x => x.Alias == result.result["alias"])}";
                    }
                    return $"?id={articles.First().Id}";
                }
            }
            return null;
        }
    }
}