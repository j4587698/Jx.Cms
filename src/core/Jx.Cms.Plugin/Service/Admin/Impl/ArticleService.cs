using System.Collections.Generic;
using Furion.DependencyInjection;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Admin.Impl
{
    public class ArticleService: IArticleService, ITransient
    {
        public ArticleEntity GetArticleById(int id)
        {
            return ArticleEntity.Select.Where(x => x.Id == id).IncludeMany(x => x.Metas).IncludeMany(x => x.Labels).First() ?? new ArticleEntity();
        }

        public List<ArticleEntity> GetAllArticle()
        {
            return ArticleEntity.Select.Where(x => x.IsPage == false).OrderByDescending(x => x.Id).ToList();
        }

        public List<ArticleEntity> GetArticlePage(int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.IsPage == false).OrderByDescending(x => x.Id).Count(out count).Page(pageNumber, pageSize).Include(x => x.Catalogue).ToList();
        }

        public List<ArticleEntity> GetArticlePage(int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.IsPage == false).OrderByDescending(x => x.Id).Include(x => x.Catalogue).Page(pageNumber, pageSize).ToList();
        }

        public List<ArticleEntity> GetArticleByLabel(string label, int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.Labels.AsSelect().Any(y => y.Name == label)).OrderByDescending(x => x.Id).Count(out count).Page(pageNumber, pageSize).ToList();
        }

        public bool SaveArticle(ArticleEntity articleEntity)
        {
            articleEntity.Save().SaveMany(nameof(ArticleEntity.Labels));
            articleEntity.SaveMany(nameof(ArticleEntity.Metas));
            return true;
        }

        public bool DeleteArticle(ArticleEntity articleEntity)
        {
            var ret = articleEntity.Delete();
            if (ret)
            {
                ArticleMetaEntity.Where(x => x.ArticleId == articleEntity.Id).ToUpdate().Set(x => x.IsDeleted, true).ExecuteAffrows();
                ArticleTagEntity.Where(x => x.ArticleId == articleEntity.Id).ToUpdate().Set(x => x.IsDeleted, true).ExecuteAffrows();
            }
            return ret;
        }
    }
}