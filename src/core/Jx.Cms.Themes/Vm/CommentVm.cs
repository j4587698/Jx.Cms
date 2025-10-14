using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Themes.Vm;

public class CommentVm
{
    /// <summary>
    ///     评论列表
    /// </summary>
    public List<CommentEntity> Comments { get; set; } = new();

    /// <summary>
    ///     总条数
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    ///     当前页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    ///     分页信息
    /// </summary>
    public Dictionary<string, int> Pagination { get; set; } = new();
}