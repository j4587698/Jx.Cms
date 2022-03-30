using System.Collections.Generic;
using System.Threading.Tasks;
using Furion.DependencyInjection;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.DbContext.Service.Admin.Impl
{
    public class PageService: IPageService, ITransient
    {
        public ArticleEntity GetPageById(int id)
        {
            return ArticleEntity.Find(id) ?? new ArticleEntity();
        }

        public List<ArticleEntity> GetAllPage()
        {
            return ArticleEntity.Select.Where(x => x.IsPage).OrderByDescending(x => x.Id).ToList();
        }

        public List<ArticleEntity> GetPageWithPage(int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.IsPage).Count(out count).OrderByDescending(x => x.Id).Page(pageNumber, pageSize).ToList();
        }

        public List<ArticleEntity> GetPageWithPage(int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.IsPage).OrderByDescending(x => x.Id).Page(pageNumber, pageSize).ToList();
        }

        public bool SavePage(ArticleEntity articleEntity)
        {
            articleEntity.Save();
            return true;
        }

        public async Task<bool> DeletePage(ArticleEntity articleEntity)
        {
            return await articleEntity.DeleteAsync();
        }
    }
}