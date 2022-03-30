using System.ComponentModel;
using FreeSql;
using FreeSql.DataAnnotations;

namespace Jx.Cms.DbContext.Entities.Article
{
    [Description("文章标签多多关系表")]
    public class ArticleTagEntity: BaseEntity<ArticleTagEntity, int>
    {
        [Column(IsPrimary = false, IsIgnore = true)]
        public override int Id { get; set; }
        
        [Description("文章ID")]
        [Column(IsPrimary = true)]
        public int ArticleId { get; set; }
        
        [Navigate(nameof(ArticleId))]
        public ArticleEntity ArticleEntity { get; set; }

        [Description("标签ID")]
        [Column(IsPrimary = true)]
        public int LabelId { get; set; }

        [Navigate(nameof(LabelId))]
        public TagEntity TagEntity { get; set; }
    }
}