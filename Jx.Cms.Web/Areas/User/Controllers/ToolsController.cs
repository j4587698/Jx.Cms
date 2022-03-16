using System.Linq;
using Furion.DataValidation;
using Jx.Cms.Common.Vo;
using Jx.Cms.DbContext.Service.Both;
using Jx.Cms.Entities.Article;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Areas.User.Controllers
{
    /// <summary>
    /// 工具类
    /// </summary>
    [Area("User")]
    [Route("/User/[action]")]
    public class ToolsController: ControllerBase
    {
        /// <summary>
        /// 提交评论
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="commentService"></param>
        /// <returns></returns>
        [NonValidation]
        [HttpPost]
        public object Comment([FromForm]CommentVo comment, [FromServices]ICommentService commentService)
        {
            var validate = comment.TryValidate();
            if (!validate.IsValid)
                return R.Fail(500002, string.Join(",", validate.ValidationResults.Select(x => x.ErrorMessage)));
            comment.AuthorIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            comment.AuthorAgent = Request.Headers["User-Agent"].ToString();
            if (!commentService.AddOrModifyComment(comment.Adapt<CommentEntity>())) return R.Fail(50001, "添加评论失败");
            Response.Cookies.Append(nameof(CommentEntity.AuthorName), comment.AuthorName);
            Response.Cookies.Append(nameof(CommentEntity.AuthorEmail), comment.AuthorEmail);
            Response.Cookies.Append(nameof(CommentEntity.AuthorUrl), comment.AuthorUrl);
            return R.Success();
        }
    }
}