using System.Collections.Generic;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;

namespace Jx.Cms.DbContext.Service.Both.Impl
{
    public class CommentService: ICommentService, ITransient
    {
        public bool AddOrModifyComment(CommentEntity commentEntity)
        {
            return commentEntity.Save() != null;
        }

        public List<CommentEntity> GetCommentPage(int pageNumber, int pageSize, out long count)
        {
            return CommentEntity.Select.Page(pageNumber, pageSize).Count(out count).ToList();
        }

        public List<CommentEntity> GetCommentPage(int pageNumber, int pageSize)
        {
            return CommentEntity.Select.Page(pageNumber, pageSize).ToList();
        }

        public List<CommentEntity> GetAllComment()
        {
            return CommentEntity.Select.ToList();
        }

        public List<CommentEntity> GetCommentPageByArticleId(int articleId, int pageNumber, int pageSize)
        {
            return CommentEntity.Where(x => x.ArticleEntity.Id == articleId).Page(pageNumber, pageSize).ToList();
        }

        public List<CommentEntity> GetCommentPageByArticleId(int articleId, int pageNumber, int pageSize, out long count)
        {
            return CommentEntity.Where(x => x.ArticleEntity.Id == articleId).Count(out count).Page(pageNumber, pageSize).ToList();
        }

        public List<CommentEntity> GetAllCommentByArticleId(int articleId)
        {
            return CommentEntity.Where(x => x.ArticleEntity.Id == articleId).ToList();
        }

        public List<CommentEntity> GetCommentTreeCteByArticleId(int articleId)
        {
            return CommentEntity.Where(x => x.ArticleId == articleId && x.ParentId == 0).AsTreeCte().OrderByDescending(x => x.CreateTime).ToTreeList();
        }
    }
}