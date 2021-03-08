using System.Collections.Generic;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;

namespace Jx.Cms.Service.Front.Impl
{
    /// <summary>
    /// 文章Service
    /// </summary>
    public class ArticleService: IArticleService, ITransient
    {
        public ArticleEntity GetArticleById(int id)
        {
            return ArticleEntity.Select.Where(x => x.Id == id).IncludeMany(x => x.Labels).First() ?? new ArticleEntity();
        }

        public ArticleEntity GetPrevArticle(int id)
        {
            return ArticleEntity.Select.Where(x => x.Id < id && x.IsPage == false).OrderByDescending(x => x.Id).First();
        }

        public ArticleEntity GetNextArticle(int id)
        {
            return ArticleEntity.Select.Where(x => x.Id > id && x.IsPage == false).OrderBy(x => x.Id).First();
        }

        public List<ArticleEntity> GetAllArticle()
        {
            return ArticleEntity.Select.Where(x => x.IsPage == false).ToList();
        }

        public List<ArticleEntity> GetArticlePageWithCount(int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.IsPage == false).Count(out count).Page(pageNumber, pageSize).Include(x => x.Catalogue).ToList();
        }

        public List<ArticleEntity> GetArticlePage(int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.IsPage == false).Include(x => x.Catalogue).Page(pageNumber, pageSize).ToList();
        }

        public List<ArticleEntity> GetArticleByLabelWithCount(string label, int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.Labels.AsSelect().Any(y => y.Name == label)).Count(out count).Page(pageNumber, pageSize).ToList();
        }

        public bool SaveArticle(ArticleEntity articleEntity)
        {
            articleEntity.Save().SaveMany("Labels");
            return true;
        }
    }
}