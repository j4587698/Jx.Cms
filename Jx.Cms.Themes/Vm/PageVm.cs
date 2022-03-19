using Jx.Cms.Entities.Article;

namespace Jx.Cms.Themes.Vm;

public class PageVm
{
    /// <summary>
    /// 文章
    /// </summary>
    public ArticleEntity Article { get; set; }

    /// <summary>
    /// 评论总条数
    /// </summary>
    public long CommentCount { get; set; }

    /// <summary>
    /// 插件扩展头
    /// </summary>
    public string HeaderExt { get; set; }

    /// <summary>
    /// 插件扩展内容
    /// </summary>
    public string BodyExt { get; set; }
}