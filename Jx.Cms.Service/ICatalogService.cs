﻿using System.Collections.Generic;
using Jx.Cms.Entities.Article;

namespace Jx.Cms.Service
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
        /// 分页获取所有的分类
        /// </summary>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="count">总数量</param>
        /// <returns>分类列表</returns>
        List<CatalogEntity> GetCatalogPageWithCount(int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 保存分类
        /// </summary>
        /// <param name="catalogEntity">分类</param>
        /// <returns>是否成功</returns>
        bool Save(CatalogEntity catalogEntity);
    }
}