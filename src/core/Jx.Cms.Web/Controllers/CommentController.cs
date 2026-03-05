using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Service.Both;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class CommentController : BaseController
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    public IActionResult Index(int id)
    {
        if (id <= 0) return NotFound();

        var article = ArticleEntity.Select
            .Where(x => x.Id == id && x.Status == ArticleStatusEnum.Published)
            .First();
        if (article == null || !article.CanComment) return NotFound();

        var comments = _commentService.GetCommentTreeCteByArticleId(id);
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
