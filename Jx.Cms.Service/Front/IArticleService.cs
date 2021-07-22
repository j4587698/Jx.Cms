using System.Collections.Generic;
using Jx.Cms.Entities.Article;
using Jx.Cms.Plugin.Model;

namespace Jx.Cms.Service.Front
{
    /// <summary>
    /// 文章服务
    /// </summary>
    public interface IArticleService
    {
        /// <summary>
        /// 根据ID获取文章
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns>对应的文章</returns>
        ArticleModel GetArticleById(int id);

        /// <summary>
        /// 根据当前文章ID获取上一篇文章，如没有，则为null
        /// </summary>
        /// <param name="id">当前文章ID</param>
        /// <returns>上一篇文章</returns>
        ArticleEntity GetPrevArticle(int id);

        /// <summary>
        /// 根据当前文章ID获取下一篇文章，如没有，则为null
        /// </summary>
        /// <param name="id">当前文章ID</param>
        /// <returns>下一篇文章</returns>
        ArticleEntity GetNextArticle(int id);

        /// <summary>
        /// 获取所有文章
        /// </summary>
        /// <returns>所有文章列表</returns>
        List<ArticleEntity> GetAllArticle();

        /// <summary>
        /// 分页获取文章
        /// </summary>
        /// <param name="pageNumber">获取第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="count">文章总数量</param>
        /// <returns>指定页文章列表</returns>
        List<ArticleEntity> GetArticlePageWithCount(int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 分页获取文章
        /// </summary>
        /// <param name="pageNumber">获取第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>制定页文章列表</returns>
        List<ArticleEntity> GetArticlePage(int pageNumber, int pageSize);

        /// <summary>
        /// 根据标签名获取文章
        /// </summary>
        /// <param name="label">标签名（如果标签名相同则会全部查出）</param>
        /// <param name="pageNumber">获取第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="count">文章总数量</param>
        /// <returns>指定页文章列表</returns>
        List<ArticleEntity> GetArticleByLabel(string label, int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 获取相关文章（根据所在分类以及相同Label）
        /// </summary>
        /// <param name="baseArticle">基准文章</param>
        /// <param name="count">获取多少个(默认10个)</param>
        /// <returns></returns>
        List<ArticleEntity> GetRelevantArticle(ArticleEntity baseArticle, int count = 10);

        /// <summary>
        /// 保存文章
        /// </summary>
        /// <param name="articleEntity">文章</param>
        /// <returns>是否成功</returns>
        bool SaveArticle(ArticleEntity articleEntity);
    }
}