using System.Collections.Generic;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Model;

namespace Jx.Cms.Plugin.Service.Front
{
    /// <summary>
    /// 页面服务
    /// </summary>
    public interface IPageService
    {
        /// <summary>
        /// 根据ID获取页面
        /// </summary>
        /// <param name="id">页面ID</param>
        /// <returns>对应的页面</returns>
        ArticleModel GetPageById(int id);

        /// <summary>
        /// 获取所有页面
        /// </summary>
        /// <returns>所有页面列表</returns>
        List<ArticleEntity> GetAllPage();

        /// <summary>
        /// 分页获取页面
        /// </summary>
        /// <param name="pageNumber">获取第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="count">页面总数量</param>
        /// <returns>指定页页面列表</returns>
        List<ArticleEntity> GetPageWithPage(int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 分页获取页面
        /// </summary>
        /// <param name="pageNumber">获取第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>制定页页面列表</returns>
        List<ArticleEntity> GetPageWithPage(int pageNumber, int pageSize);
        
    }
}