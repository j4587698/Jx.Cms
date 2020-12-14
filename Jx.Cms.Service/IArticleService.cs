using Jx.Cms.Entities.Admin;

namespace Jx.Cms.Service
{
    public interface IArticleService
    {
        ArticleEntity GetArticleById(int id);
    }
}