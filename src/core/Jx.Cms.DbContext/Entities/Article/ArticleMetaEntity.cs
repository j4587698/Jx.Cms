using System.ComponentModel;
using FreeSql;
using FreeSql.DataAnnotations;

namespace Jx.Cms.DbContext.Entities.Article;

[Description("文章扩展项")]
public class ArticleMetaEntity : BaseEntity<ArticleMetaEntity, long>
{
    [Description("文章Id")]
    public int ArticleId { get; set; }

    [Navigate(nameof(ArticleId))]
    public ArticleEntity Article { get; set; }

    [Description("所属插件名")]
    public string PluginName { get; set; }

    [Description("项")]
    public string Name { get; set; }

    [Description("值")]
    public string Value { get; set; }
}