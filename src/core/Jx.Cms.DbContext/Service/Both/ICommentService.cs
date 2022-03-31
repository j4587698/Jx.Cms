using System.Collections.Generic;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.DbContext.Service.Both
{
    /// <summary>
    /// 评论相关
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// 添加或修改评论
        /// </summary>
        /// <param name="commentEntity"></param>
        /// <returns></returns>
        bool AddOrModifyComment(CommentEntity commentEntity);

        /// <summary>
        /// 分页获取评论
        /// </summary>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        List<CommentEntity> GetCommentPage(int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<CommentEntity> GetCommentPage(int pageNumber, int pageSize);

        /// <summary>
        /// 获取所有评论
        /// </summary>
        /// <returns></returns>
        List<CommentEntity> GetAllComment();

        /// <summary>
        /// 分页获取某个文章的评论内容
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        List<CommentEntity> GetCommentPageByArticleId(int articleId, int pageNumber, int pageSize);

        /// <summary>
        /// 分页获取某个文章的评论内容
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        List<CommentEntity> GetCommentPageByArticleId(int articleId, int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 获取某个文章下的所有评论
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <returns></returns>
        List<CommentEntity> GetAllCommentByArticleId(int articleId);

        /// <summary>
        /// 以树形获取某个文章下的评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        List<CommentEntity> GetCommentTreeCteByArticleId(int articleId);

        /// <summary>
        /// 切换评论审核状态
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="commentStatus"></param>
        void ChangeStatus(int commentId, CommentStatusEnum commentStatus);

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentEntity"></param>
        bool DeleteComment(CommentEntity commentEntity);
    }
}