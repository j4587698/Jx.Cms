using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Blogs.Model;
using Jx.Cms.Entities.Article;
using Jx.Cms.Rewrite;
using Masuit.Tools;
using Masuit.Tools.Strings;

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
                .Set("category", articleEntity.Catalogue.Alias)
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

                Expression<Func<ArticleEntity, bool>> where = x => x.IsPage == false;
                foreach (var info in result.result)
                {
                    switch (info.Key)
                    {
                        case "year":
                            where = where.And(x => x.PublishTime.Year.ToString() == info.Value);
                            break;
                        case "month":
                            var month = info.Value.PadLeft(2, '0');
                            where = where.And(x => x.PublishTime.Month.ToString() == month);
                            break;
                        case "day":
                            var day = info.Value.PadLeft(2, '0');
                            where = where.And(x => x.PublishTime.Day.ToString() == day);
                            break;
                        case "alias":
                            where = where.And(x =>
                                x.Alias == result.result["alias"] || x.Title == result.result["alias"]);
                            break;
                        case "category":
                            where = where.And(x => x.Catalogue.Alias == info.Value);
                            break;
                    }
                }

                var sql = ArticleEntity.Select.Where(where).ToSql();
                var articles = ArticleEntity.Select.Where(where);
                if (articles == null || articles.Count() == 0)
                {
                    return null;
                }

                return $"?id={articles.First().Id}";
            }
            return null;
        }
    }
}