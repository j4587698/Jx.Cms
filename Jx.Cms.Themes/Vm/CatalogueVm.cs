using System.Collections.Generic;
using Jx.Cms.Entities.Article;

namespace Jx.Cms.Themes.Vm;

public class CatalogueVm
{
    /// <summary>
    /// 分类目录
    /// </summary>
    public CatalogEntity Catalogue { get; set; }
    
    /// <summary>
    /// 文章列表
    /// </summary>
    public List<ArticleEntity> Articles { get; set; }
    
    /// <summary>
    /// 文章总数量
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageNum { get; set; }

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// 页码信息
    /// </summary>
    public Dictionary<string, int> Pagination { get; set; }
}