using System.Collections.Generic;
using System.Threading.Tasks;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;

namespace Jx.Cms.Service.Admin.Impl
{
    public class PageService: IPageService, ITransient
    {
        public ArticleEntity GetArticleById(int id)
        {
            return ArticleEntity.Find(id) ?? new ArticleEntity();
        }

        public List<ArticleEntity> GetAllArticle()
        {
            return ArticleEntity.Select.Where(x => x.IsPage).ToList();
        }

        public List<ArticleEntity> GetArticlePageWithCount(int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.IsPage).Count(out count).Page(pageNumber, pageSize).ToList();
        }

        public List<ArticleEntity> GetArticlePage(int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.IsPage).Page(pageNumber, pageSize).ToList();
        }

        public bool SaveArticle(ArticleEntity articleEntity)
        {
            articleEntity.Save();
            return true;
        }

        public async Task<bool> DeleteArticle(ArticleEntity articleEntity)
        {
            return await articleEntity.DeleteAsync();
        }
    }
}