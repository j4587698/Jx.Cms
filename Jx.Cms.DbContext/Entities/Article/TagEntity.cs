using System.Collections.Generic;
using System.ComponentModel;
using FreeSql;
using FreeSql.DataAnnotations;

namespace Jx.Cms.DbContext.Entities.Article
{
    [Description("标签")]
    public class TagEntity: BaseEntity<TagEntity, int>
    {
        [Description("标签名")]
        public string Name { get; set; }

        [Description("文章列表")]
        [Navigate(ManyToMany = typeof(ArticleTagEntity))]
        public List<ArticleEntity> Articles { get; set; }
    }
}