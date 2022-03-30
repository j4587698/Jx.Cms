using System.Collections.Generic;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Front;

/// <summary>
/// 前端分类目录服务
/// </summary>
public interface ICatalogService
{
    /// <summary>
    /// 根据Id获取分类目录信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    CatalogEntity GetCatalogById(int id);

    /// <summary>
    /// 根据分类目录Id获取下面所有文章
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includeChildren"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<ArticleEntity> GetArticlesByCatalogId(int id, bool includeChildren, int pageNumber, int pageSize, out long count);
}