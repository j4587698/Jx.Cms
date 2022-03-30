using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Jx.Cms.Common.Vo
{
    public class CommentVo
    {
        [Description("文章ID")]
        [Required(ErrorMessage = "所属文章不能为空")]
        public int ArticleId { get; set; }

        [Description("作者名")]
        [Required(ErrorMessage = "评论者名不能为空")]
        public string AuthorName { get; set; }

        [Description("作者Email")]
        [Required(ErrorMessage = "评论者Email不能为空")]
        public string AuthorEmail { get; set; }

        [Description("作者地址")]
        public string AuthorUrl { get; set; }

        [Description("作者IP")]
        public string AuthorIp { get; set; }

        [Description("作者浏览器Agent")]
        public string AuthorAgent { get; set; }
        
        [Description("父ID")]
        public int ParentId { get; set; }

        [Description("根评论Id")]
        public int RootId { get; set; }

        [Description("内容")]
        [Required(ErrorMessage = "评论内容不能为空")]
        public string Content { get; set; }
    }
}