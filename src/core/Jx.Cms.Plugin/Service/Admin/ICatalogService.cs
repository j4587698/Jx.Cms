using System.Collections.Generic;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Admin
{
    /// <summary>
    /// 分类目录
    /// </summary>
    public interface ICatalogService
    {

        /// <summary>
        /// 根据ID获取分类
        /// </summary>
        /// <param name="id">分类ID</param>
        /// <returns>分类</returns>
        CatalogEntity FindCatalogById(int id);

        /// <summary>
        /// 获取所有的分类
        /// </summary>
        /// <returns>分类列表</returns>
        List<CatalogEntity> GetAllCatalogs();

        /// <summary>
        /// 分页获取所有的分类
        /// </summary>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="count">总数量</param>
        /// <returns>分类列表</returns>
        List<CatalogEntity> GetCatalogPage(int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 保存分类
        /// </summary>
        /// <param name="catalogEntity">分类</param>
        /// <returns>是否成功</returns>
        bool Save(CatalogEntity catalogEntity);

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="catalogEntities">要删除的分类列表</param>
        /// <returns>是否成功</returns>
        bool Delete(IEnumerable<CatalogEntity> catalogEntities);
    }
}