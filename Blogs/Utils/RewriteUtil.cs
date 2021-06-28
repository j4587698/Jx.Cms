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
        /// <summary>
        /// 获取文章页伪静态Url
        /// </summary>
        /// <param name="articleEntity">文章信息</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取页面伪静态Url
        /// </summary>
        /// <param name="articleEntity"></param>
        /// <returns></returns>
        public static string GetPageUrl(ArticleEntity articleEntity)
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return $"/Page?id={articleEntity.Id}";
            }
            var template = Template.Create(rewriterModel.PageUrl);
            return template.Set("id", articleEntity.Id.ToString()).Set("year", articleEntity.PublishTime.Year.ToString())
                .Set("month", articleEntity.PublishTime.Month.ToString())
                .Set("day", articleEntity.PublishTime.Day.ToString())
                .Set("category", articleEntity.Catalogue.Alias)
                .Set("alias", articleEntity.Alias.IsNullOrEmpty() ? articleEntity.Title : articleEntity.Alias).Render();
        }

        /// <summary>
        /// 获取首页Url
        /// </summary>
        /// <param name="pageNo">当前为第几页</param>
        /// <returns></returns>
        public static string GetIndexUrl(int pageNo)
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return $"/Index?page={pageNo}";
            }
            var template = Template.Create(rewriterModel.IndexUrl);
            return template.Set("page", pageNo.ToString()).Render();
        }
        
        private static List<string> _articleList;

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
                
                var articles = ArticleEntity.Select.Where(where);
                if (articles == null || articles.Count() == 0)
                {
                    return null;
                }

                return $"?id={articles.First().Id}";
            }
            return null;
        }
        
        private static List<string> _pageList;

        public static string AnalysisPage(string url)
        {
            var rewriterModel = RewriterModel.GetSettings();
            _pageList ??= RewriteTemplate.CreateUrl(rewriterModel.PageUrl);
            if (_pageList == null || _pageList.Count == 0)
            {
                _pageList = null;
                return null;
            }
            
            var result = RewriteTemplate.AnalysisUrl(url, _pageList);
            if (result.isSuccess)
            {
                if (result.result.ContainsKey("id"))
                {
                    return $"?id={result.result["id"]}";
                }
                Expression<Func<ArticleEntity, bool>> where = x => x.IsPage == true;
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
                
                var pages = ArticleEntity.Select.Where(where);
                if (pages == null || pages.Count() == 0)
                {
                    return null;
                }

                return $"?id={pages.First().Id}";
            }

            return null;
        }
        
        private static List<string> _indexList;

        public static string AnalysisIndex(string url)
        {
            var rewriterModel = RewriterModel.GetSettings();
            _indexList ??= RewriteTemplate.CreateUrl(rewriterModel.IndexUrl);
            if (_indexList == null || _indexList.Count == 0)
            {
                _indexList = null;
                return null;
            }
            var result = RewriteTemplate.AnalysisUrl(url, _indexList);
            if (result.isSuccess)
            {
                return $"?page={result.result["page"]}";
            }
            return null;
        }
    }
}