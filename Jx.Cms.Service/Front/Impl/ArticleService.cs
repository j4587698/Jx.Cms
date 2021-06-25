using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Utils;
using Markdig;

namespace Jx.Cms.Service.Front.Impl
{
    /// <summary>
    /// 文章Service
    /// </summary>
    public class ArticleService: IArticleService, ITransient
    {
        public ArticleModel GetArticleById(int id)
        {
            var article = ArticleEntity.Select.Where(x => x.Id == id).IncludeMany(x => x.Comments).IncludeMany(x => x.Labels).First() ?? new ArticleEntity();
            if (article.IsMarkdown)
            {
                article.Content = Markdown.ToHtml(article.Content);
            }
            article.ReadingVolume = article.ReadingVolume + 1;
            ArticleEntity.Where(x => x.Id == id).ToUpdate().Set(x => x.ReadingVolume, article.ReadingVolume);
            var model = new ArticleModel();
            model.Body = article;
            PluginUtil.OnArticleShow(model);
            return model;
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
            return ArticleEntity.Select.Where(x => x.IsPage == false).Include(x => x.Catalogue).IncludeMany(x => x.Comments.Select(y => new CommentEntity(){Id = y.Id})).OrderByDescending(x => x.PublishTime).ToList();
        }

        public List<ArticleEntity> GetArticlePageWithCount(int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.IsPage == false).OrderByDescending(x => x.PublishTime).Count(out count).Page(pageNumber, pageSize).Include(x => x.Catalogue).IncludeMany(x => x.Comments.Select(y => new CommentEntity(){Id = y.Id})).ToList();
        }

        public List<ArticleEntity> GetArticlePage(int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.IsPage == false).OrderByDescending(x => x.PublishTime).Include(x => x.Catalogue).Page(pageNumber, pageSize).IncludeMany(x => x.Comments.Select(y => new CommentEntity(){Id = y.Id})).ToList();
        }

        public List<ArticleEntity> GetArticleByLabelWithCount(string label, int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.Labels.AsSelect().Any(y => y.Name == label)).Count(out count).OrderByDescending(x => x.PublishTime).Page(pageNumber, pageSize).IncludeMany(x => x.Comments.Select(y => new CommentEntity(){Id = y.Id})).ToList();
        }

        public bool SaveArticle(ArticleEntity articleEntity)
        {
            articleEntity.Save().SaveMany("Labels");
            return true;
        }
    }
}