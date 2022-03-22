using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Rewrite;
using Jx.Cms.Themes.Model;
using Masuit.Tools;
using Masuit.Tools.Strings;

namespace Jx.Cms.Themes.Util
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
            if (rewriterModel.RewriteOption.IsNullOrEmpty() || rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return $"/Post?id={articleEntity.Id}";
            }

            var template = Template.Create(rewriterModel.ArticleUrl);
            return template.Set("id", articleEntity.Id.ToString()).Set("year", articleEntity.PublishTime.Year.ToString())
                .Set("month", articleEntity.PublishTime.Month.ToString())
                .Set("day", articleEntity.PublishTime.Day.ToString())
                .Set("category", articleEntity.Catalogue == null ? "" : articleEntity.Catalogue.Alias.IsNullOrEmpty() ? articleEntity.Catalogue.Name : articleEntity.Catalogue.Alias)
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
            if (rewriterModel.RewriteOption.IsNullOrEmpty() || rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
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
        public static string GetIndexUrl(long pageNo = 1)
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel.RewriteOption.IsNullOrEmpty() || rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return $"/?pageNum={pageNo}";
            }
            var template = Template.Create(rewriterModel.IndexUrl);
            return template.Set("page", pageNo.ToString()).Render();
        }

        /// <summary>
        /// 获取标签页Url
        /// </summary>
        /// <param name="tagel">标签</param>
        /// <param name="pageNo">第几页</param>
        /// <returns></returns>
        public static string GetTagUrl(TagEntity tag, long pageNo = 1)
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel.RewriteOption.IsNullOrEmpty() || rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return $"/Label?id={tag.Id}&pageNum={pageNo}";
            }

            var template = Template.Create(rewriterModel.LabelUrl);
            return template.Set("id", tag.Id.ToString()).Set("page", pageNo.ToString()).Set("alias", tag.Name).Render();
        }

        /// <summary>
        /// 获取分类标签
        /// </summary>
        /// <param name="catalogEntity"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public static string GetCatalogUrl(CatalogEntity catalogEntity, long pageNo = 1)
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel.RewriteOption.IsNullOrEmpty() || rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return $"/Catalogue?id={catalogEntity.Id}&pageNum={pageNo}";
            }
            var template = Template.Create(rewriterModel.CatalogueUrl);
            return template.Set("id", catalogEntity.Id.ToString()).Set("page", pageNo.ToString()).Set("alias", catalogEntity.Alias.IsNullOrEmpty() ? catalogEntity.Name : catalogEntity.Alias).Render();
        }

        public static string GetDateUrl(int year, int month, long pageNo)
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel.RewriteOption.IsNullOrEmpty() || rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return $"/Date?year={year}&month={month}&pageNum={pageNo}";
            }
            var template = Template.Create(rewriterModel.DateUrl);
            return template.Set("year", year.ToString()).Set("month", month.ToString())
                .Set("pageNum", pageNo.ToString()).Render();
        }

        public static string AnalysisArticle(string url, RewriterModel rewriterModel)
        {
            List<string> articleList = RewriteTemplate.CreateUrl(rewriterModel.ArticleUrl);

            if (articleList == null || articleList.Count == 0)
            {
                return null;
            }

            var result = RewriteTemplate.AnalysisUrl(url, articleList);
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
                            where = where.And(x => x.PublishTime.Year == info.Value.ConvertTo<int>());
                            break;
                        case "month":
                            where = where.And(x => x.PublishTime.Month == info.Value.ConvertTo<int>());
                            break;
                        case "day":
                            where = where.And(x => x.PublishTime.Day == info.Value.ConvertTo<int>());
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

        public static string AnalysisPage(string url, RewriterModel rewriterModel)
        {
            List<string> pageList = RewriteTemplate.CreateUrl(rewriterModel.PageUrl);
            if (pageList == null || pageList.Count == 0)
            {
                return null;
            }
            
            var result = RewriteTemplate.AnalysisUrl(url, pageList);
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
        

        public static string AnalysisIndex(string url, RewriterModel rewriterModel)
        {
            List<string> indexList = RewriteTemplate.CreateUrl(rewriterModel.IndexUrl);
            if (indexList == null || indexList.Count == 0)
            {
                return null;
            }
            var result = RewriteTemplate.AnalysisUrl(url, indexList);
            if (result.isSuccess)
            {
                return $"?pageNum={result.result["page"]}";
            }
            return null;
        }

        public static string AnalysisTag(string url, RewriterModel rewriterModel)
        {
            List<string> labelList = RewriteTemplate.CreateUrl(rewriterModel.LabelUrl);
            if (labelList == null || labelList.Count == 0)
            {
                return null;
            }

            var result = RewriteTemplate.AnalysisUrl(url, labelList);

            if (result.isSuccess)
            {
                if (!result.result.ContainsKey("page"))
                {
                    return null;
                }

                if (result.result.ContainsKey("id"))
                {
                    return $"?id={result.result["id"]}&pageNum={result.result["page"]}";
                }

                if (result.result.ContainsKey("alias"))
                {
                    var labelEntity = TagEntity.Where(x => x.Name == result.result["alias"]).First();
                    return labelEntity == null ? null : $"?id={labelEntity.Id}&pageNum={result.result["page"]}";
                }
            }

            return null;
        }
        
        public static string AnalysisCatalog(string url, RewriterModel rewriterModel)
        {
            List<string> catalogueList = RewriteTemplate.CreateUrl(rewriterModel.CatalogueUrl);
            if (catalogueList == null || catalogueList.Count == 0)
            {
                return null;
            }

            var result = RewriteTemplate.AnalysisUrl(url, catalogueList);

            if (result.isSuccess)
            {
                if (!result.result.ContainsKey("page"))
                {
                    return null;
                }

                if (result.result.ContainsKey("id"))
                {
                    return $"?id={result.result["id"]}&pageNum={result.result["page"]}";
                }

                if (result.result.ContainsKey("alias"))
                {
                    var catalogEntity = CatalogEntity.Where(x => x.Alias == result.result["alias"] || x.Name == result.result["alias"]).First();
                    return catalogEntity == null ? null : $"?id={catalogEntity.Id}&pageNum={result.result["page"]}";
                }
            }

            return null;
        }

        public static string AnalysisDate(string url, RewriterModel rewriterModel)
        {
            List<string> dateList = RewriteTemplate.CreateUrl(rewriterModel.DateUrl);
            if (dateList == null || dateList.Count == 0)
            {
                return null;
            }

            var result = RewriteTemplate.AnalysisUrl(url, dateList);

            if (result.isSuccess)
            {
                if (!(result.result.ContainsKey("pageNum") && result.result.ContainsKey("year") && result.result.ContainsKey("month")))
                {
                    return null;
                }

                return
                    $"?year={result.result.ContainsKey("year")}&month={result.result.ContainsKey("month")}&pageNum={result.result.ContainsKey("pageNum")}";
            }

            return null;
        }
    }
}