using System.Collections.Generic;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;

namespace Jx.Cms.Service.Admin.Impl
{
    public class ArticleService: IArticleService, ITransient
    {
        public ArticleEntity GetArticleById(int id)
        {
            return ArticleEntity.Find(id) ?? new ArticleEntity();
        }

        public List<ArticleEntity> GetAllArticle()
        {
            return ArticleEntity.Select.ToList();
        }

        public List<ArticleEntity> GetArticlePageWithCount(int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Count(out count).Page(pageNumber, pageSize).Include(x => x.Catalogue).ToList();
        }

        public List<ArticleEntity> GetArticlePage(int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Include(x => x.Catalogue).Page(pageNumber, pageSize).ToList();
        }

        public bool SaveArticle(ArticleEntity articleEntity)
        {
            articleEntity.Save().SaveMany("Labels");
            return true;
        }
    }
}