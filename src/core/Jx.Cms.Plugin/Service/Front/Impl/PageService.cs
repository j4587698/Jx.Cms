using System.Collections.Generic;
using Furion.DependencyInjection;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Utils;
using Markdig;

namespace Jx.Cms.Plugin.Service.Front.Impl
{
    public class PageService: IPageService, ITransient
    {
        public ArticleModel GetPageById(int id)
        {
            var article = ArticleEntity.Select.Where(x => x.Status == ArticleStatusEnum.Published && x.Id == id && x.IsPage).First();
            if (article == null)
            {
                return null;
            }
            if (article.IsMarkdown)
            {
                article.Content = Markdown.ToHtml(article.Content);
            }

            article.Comments = CommentEntity.Where(x => x.ParentId == 0 && x.ArticleId == article.Id && x.Status == CommentStatusEnum.Pass).AsTreeCte().Count(out var count).ToTreeList();
            
            article.ReadingVolume += 1;
            ArticleEntity.Where(x => x.Id == id).ToUpdate().Set(x => x.ReadingVolume, article.ReadingVolume).ExecuteAffrows();
            var model = new ArticleModel
            {
                Body = article,
                CommentCount = count
            };
            PluginUtil.OnArticleShow(model);
            return model;
        }

        public List<ArticleEntity> GetAllPage()
        {
            return ArticleEntity.Select.Where(x => x.Status == ArticleStatusEnum.Published && x.IsPage).ToList();
        }

        public List<ArticleEntity> GetPageWithPage(int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.Status == ArticleStatusEnum.Published && x.IsPage).Count(out count).Page(pageNumber, pageSize).Include(x => x.Catalogue).ToList();
        }

        public List<ArticleEntity> GetPageWithPage(int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.Status == ArticleStatusEnum.Published && x.IsPage).Include(x => x.Catalogue).Page(pageNumber, pageSize).ToList();
        }
    }
}