using System;
using System.ComponentModel;
using FreeSql;

namespace Jx.Cms.Entities.Article
{
    [Description("文章标签对照")]
    public class ArticleLabel:BaseEntity<ArticleEntity, Guid>
    {
        [Description("文章ID")]
        public int ArticleId { get; set; }

        [Description("文章类")]
        public virtual ArticleEntity Article { get; set; }

        [Description("标签ID")]
        public int LabelId { get; set; }

        [Description("标签类")]
        public virtual LabelEntity Label { get; set; }
    }
}