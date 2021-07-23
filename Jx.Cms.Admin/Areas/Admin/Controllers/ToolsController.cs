using Furion.DataValidation;
using Jx.Cms.Common.Vo;
using Jx.Cms.Entities.Article;
using Jx.Cms.Service.Both;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Admin.Areas.Admin.Controllers
{
    public class ToolsController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return null;
        }

        [NonValidation]
        public IActionResult Comment([FromForm]CommentVo comment, [FromServices]ICommentService commentService)
        {
            var validate = comment.TryValidate();
            if (validate.IsValid)
            {
                comment.AuthorIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                comment.AuthorAgent = Request.Headers["User-Agent"].ToString();
                var ret = commentService.AddOrModifyComment(comment.Adapt<CommentEntity>());
                if (ret)
                {
                    Response.Cookies.Append(nameof(CommentEntity.AuthorName), comment.AuthorName);
                    Response.Cookies.Append(nameof(CommentEntity.AuthorEmail), comment.AuthorEmail);
                    Response.Cookies.Append(nameof(CommentEntity.AuthorUrl), comment.AuthorUrl);
                    //return JsonResult
                }
            }

            return null;

        }

    }
}