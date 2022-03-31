using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FreeSql;
using FreeSql.DataAnnotations;
using Jx.Cms.Common.Enum;

namespace Jx.Cms.DbContext.Entities.Article
{
    [Description("评论")]
    public class CommentEntity: BaseEntity<CommentEntity, int>
    {
        [Description("文章ID")]
        public int ArticleId { get; set; }

        [Navigate(nameof(ArticleId))]
        public virtual ArticleEntity ArticleEntity { get; set; }

        [Description("作者名")]
        public string AuthorName { get; set; }

        [Description("作者Email")]
        public string AuthorEmail { get; set; }

        [Description("作者地址")]
        public string AuthorUrl { get; set; }

        [Description("作者IP")]
        public string AuthorIp { get; set; }

        [Description("作者浏览器Agent")]
        public string AuthorAgent { get; set; }
        
        [Description("父ID")]
        public int ParentId { get; set; }

        [Description("父评论")]
        [Navigate(nameof(ParentId))]
        public CommentEntity Parent { get; set; }
        
        [Description("子评论")]
        [Navigate(nameof(ParentId))]
        public List<CommentEntity> Child { get; set; }

        [Description("根评论Id")]
        public int RootId { get; set; }

        [Description("根评论")]
        [Navigate(nameof(RootId))]
        public CommentEntity Root { get; set; }

        [Description("内容")]
        [MaxLength(2048)]
        public string Content { get; set; }

        [Description("评论状态")]
        public CommentStatusEnum Status { get; set; }
    }
}