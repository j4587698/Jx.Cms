﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Utils;
using Markdig;
using Masuit.Tools.Models;

namespace Jx.Cms.Service.Front.Impl
{
    /// <summary>
    /// 文章Service
    /// </summary>
    public class ArticleService: IArticleService, ITransient
    {
        public ArticleModel GetArticleById(int id)
        {
            var article = ArticleEntity.Select.Where(x => x.Id == id && !x.IsPage).Include(x => x.Catalogue).IncludeMany(x => x.Labels).First();
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

        public ArticleEntity GetPrevArticle(int id)
        {
            return ArticleEntity.Select.Where(x => x.Id < id && x.IsPage == false).Include(x => x.Catalogue).OrderByDescending(x => x.Id).First();
        }

        public ArticleEntity GetNextArticle(int id)
        {
            return ArticleEntity.Select.Where(x => x.Id > id && x.IsPage == false).Include(x => x.Catalogue).OrderBy(x => x.Id).First();
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

        public List<ArticleEntity> GetArticleByLabel(string label, int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.Labels.AsSelect().Any(y => y.Name == label)).Count(out count).OrderByDescending(x => x.PublishTime).Page(pageNumber, pageSize).IncludeMany(x => x.Comments.Select(y => new CommentEntity(){Id = y.Id})).ToList();
        }

        public List<ArticleEntity> GetRelevantArticle(ArticleEntity baseArticle, int count = 10)
        {
            List<ArticleEntity> articles = new List<ArticleEntity>();
            if (baseArticle.Labels is {Count: > 0})
            {
                var ids = baseArticle.Labels.Select(x => x.Id).ToList();
                var articleIds = ArticleLabelEntity.Where(x => ids.Contains(x.LabelId)).GroupBy(x => x.ArticleId).ToList(x => x.Key);
                articles.AddRange(ArticleEntity.Where(x => articleIds.Contains(x.Id) && x.IsPage == false).Include(x => x.Catalogue).Take(count).ToList());
            }

            if (articles.Count < count)
            {
               articles.AddRange(ArticleEntity.Where(x => !x.IsPage && x.CatalogueId == baseArticle.CatalogueId).Include(x => x.Catalogue).Take(count - articles.Count).ToList());
            }

            return articles;
        }

        public bool SaveArticle(ArticleEntity articleEntity)
        {
            articleEntity.Save().SaveMany("Labels");
            return true;
        }
    }
}