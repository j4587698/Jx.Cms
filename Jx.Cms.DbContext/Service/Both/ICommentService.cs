using System.Collections.Generic;
using Jx.Cms.Entities.Article;

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

        List<CommentEntity> GetCommentTreeCteByArticleId(int articleId);
    }
}