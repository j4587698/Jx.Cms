using System.ComponentModel;
using FreeSql;
using Jx.Cms.Common.Enum;

namespace Jx.Cms.DbContext.Entities.Settings
{
    [Description("伪静态规则")]
    public class RewriteEntity:BaseEntity<RewriteEntity, int>
    {
        [Description("伪静态类型")]
        public RewriteTypeEnum Type { get; set; }

        [Description("文章页URL")]
        public string ArticleUrl { get; set; }

        [Description("页面URL")]
        public string PageUrl { get; set; }

        [Description("首页URL")]
        public string HomeUrl { get; set; }

        [Description("分类页URL")]
        public string CatalogUrl { get; set; }

        [Description("标签页URL")]
        public string TagUrl { get; set; }

        [Description("作者页URL")]
        public string AuthorUrl { get; set; }
    }
}