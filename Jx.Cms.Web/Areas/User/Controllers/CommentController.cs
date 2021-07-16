using Jx.Cms.Entities.Article;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Areas.User.Controllers
{
    [Area("User")]
    public class CommentController: Controller
    {
        public IActionResult PostComment(CommentEntity commentEntity)
        {
            return null;
        }
    }
}