using System.Collections.Generic;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Utils;
using Markdig;

namespace Jx.Cms.Service.Front.Impl
{
    public class PageService: IPageService, ITransient
    {
        public ArticleModel GetPageById(int id)
        {
            var article = ArticleEntity.Select.Where(x => x.Id == id && x.IsPage).First();
            if (article == null)
            {
                return null;
            }
            if (article.IsMarkdown)
            {
                article.Content = Markdown.ToHtml(article.Content);
            }

            article.Comments = CommentEntity.Where(x => x.ParentId == 0 && x.ArticleId == article.Id).AsTreeCte().ToTreeList();
            //article.Comments.ToTreeGeneral(x => x.Id, x => x.ParentId);
            article.ReadingVolume += 1;
            ArticleEntity.Where(x => x.Id == id).ToUpdate().Set(x => x.ReadingVolume, article.ReadingVolume);
            var model = new ArticleModel
            {
                Body = article
            };
            PluginUtil.OnArticleShow(model);
            return model;
        }

        public List<ArticleEntity> GetAllPage()
        {
            return ArticleEntity.Select.Where(x => x.IsPage).ToList();
        }

        public List<ArticleEntity> GetPageWithPage(int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.IsPage).Count(out count).Page(pageNumber, pageSize).Include(x => x.Catalogue).ToList();
        }

        public List<ArticleEntity> GetPageWithPage(int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.IsPage).Include(x => x.Catalogue).Page(pageNumber, pageSize).ToList();
        }
    }
}