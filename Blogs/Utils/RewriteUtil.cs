using System.Collections.Generic;
using Blogs.Model;
using Jx.Cms.Entities.Article;
using Jx.Cms.Rewrite;
using Microsoft.AspNetCore.Rewrite;

namespace Blogs.Utils
{
    public class RewriteUtil
    {
        private static List<string> _articleList;
        public static string GetArticleUrl(ArticleEntity articleEntity)
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return $"/Article?id={articleEntity.Id}";
            }

            if (_articleList == null)
            {
                _articleList = RewriteTemplate.CreateUrl(rewriterModel.ArticleUrl);
            }
            return "";
        }
    }
}