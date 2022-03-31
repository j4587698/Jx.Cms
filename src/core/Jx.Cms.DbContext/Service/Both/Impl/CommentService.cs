using System.Collections.Generic;
using FreeSql;
using Furion.DependencyInjection;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;

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
            return CommentEntity.Select.OrderByDescending(x => x.Status).OrderByDescending(x => x.Id).Count(out count).Page(pageNumber, pageSize).ToList();
        }

        public List<CommentEntity> GetCommentPage(int pageNumber, int pageSize)
        {
            return CommentEntity.Select.OrderByDescending(x => x.Status).OrderByDescending(x => x.Id).Page(pageNumber, pageSize).Include(x => x.ArticleEntity).ToList();
        }

        public List<CommentEntity> GetAllComment()
        {
            return CommentEntity.Select.OrderByDescending(x => x.Status).OrderByDescending(x => x.Id).Include(x => x.ArticleEntity).ToList();
        }

        public List<CommentEntity> GetCommentPageByArticleId(int articleId, int pageNumber, int pageSize)
        {
            return CommentEntity.Where(x => x.ArticleEntity.Id == articleId && x.Status == CommentStatusEnum.Pass).OrderByDescending(x => x.Id).Page(pageNumber, pageSize).ToList();
        }

        public List<CommentEntity> GetCommentPageByArticleId(int articleId, int pageNumber, int pageSize, out long count)
        {
            return CommentEntity.Where(x => x.ArticleEntity.Id == articleId && x.Status == CommentStatusEnum.Pass).OrderByDescending(x => x.Id).Count(out count).Page(pageNumber, pageSize).ToList();
        }

        public List<CommentEntity> GetAllCommentByArticleId(int articleId)
        {
            return CommentEntity.Where(x => x.ArticleEntity.Id == articleId && x.Status == CommentStatusEnum.Pass).OrderByDescending(x => x.Id).ToList();
        }

        public List<CommentEntity> GetCommentTreeCteByArticleId(int articleId)
        {
            return CommentEntity.Where(x => x.ArticleId == articleId && x.ParentId == 0 && x.Status == CommentStatusEnum.Pass).OrderByDescending(x => x.Id).AsTreeCte().OrderByDescending(x => x.CreateTime).ToTreeList();
        }

        public void ChangeStatus(int commentId, CommentStatusEnum commentStatus)
        {
            BaseEntity.Orm.Update<CommentEntity>().Where(x => x.Id == commentId).Set(x => x.Status, commentStatus).ExecuteAffrows();
        }

        public bool DeleteComment(CommentEntity commentEntity)
        {
            return commentEntity.Delete();
        }
    }
}