using System.Collections.Generic;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Themes.Vm;

public class PostVm
{
    /// <summary>
    /// 文章
    /// </summary>
    public ArticleEntity Article { get; set; }

    /// <summary>
    /// 总评论数
    /// </summary>
    public long CommentCount { get; set; }

    /// <summary>
    /// 前一篇文章
    /// </summary>
    public ArticleEntity PrevArticle { get; set; }

    /// <summary>
    /// 下一篇文章
    /// </summary>
    public ArticleEntity NextArticle { get; set; }
    
    /// <summary>
    /// 相关文章列表
    /// </summary>
    public List<ArticleEntity> Relevant { get; set; }

    /// <summary>
    /// 插件扩展头
    /// </summary>
    public string HeaderExt { get; set; }

    /// <summary>
    /// 插件扩展内容
    /// </summary>
    public string BodyExt { get; set; }
}