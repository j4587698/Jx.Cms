using Furion.DependencyInjection;
using Jx.Cms.Entities.Admin;

namespace Jx.Cms.Service.Impl
{
    public class ArticleService: IArticleService, ITransient
    {
        public ArticleEntity GetArticleById(int id)
        {
            return ArticleEntity.Find(id) ?? new ArticleEntity();
        }
    }
}