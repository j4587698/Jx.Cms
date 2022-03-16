using System.Collections.Generic;
using System.ComponentModel;
using FreeSql;
using FreeSql.DataAnnotations;

namespace Jx.Cms.Entities.Article
{
    [Description("标签")]
    public class LabelEntity: BaseEntity<LabelEntity, int>
    {
        [Description("标签名")]
        public string Name { get; set; }

        [Description("文章列表")]
        [Navigate(ManyToMany = typeof(ArticleLabelEntity))]
        public List<ArticleEntity> Articles { get; set; }
    }
}