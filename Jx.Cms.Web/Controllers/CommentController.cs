using Furion;
using Jx.Cms.Entities.Article;
using Jx.Cms.Service.Both;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class CommentController : BaseController
{
    public IActionResult Index(int id)
    {
        var comments = App.GetService<ICommentService>().GetCommentTreeCteByArticleId(id);
        Request.Cookies.TryGetValue(nameof(CommentEntity.AuthorName), out var nikeName);
        Request.Cookies.TryGetValue(nameof(CommentEntity.AuthorEmail), out var email);
        Request.Cookies.TryGetValue(nameof(CommentEntity.AuthorUrl), out var url);
        ViewData[nameof(CommentEntity.AuthorName)] = nikeName;
        ViewData[nameof(CommentEntity.AuthorEmail)] = email;
        ViewData[nameof(CommentEntity.AuthorUrl)] = url;
        ViewData[nameof(CommentEntity.ArticleId)] = id;
        return PartialView(comments);
    }
}