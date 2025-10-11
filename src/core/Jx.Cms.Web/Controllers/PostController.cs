using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Service.Front.Impl;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class PostController : BaseController
{
    // GET
    public IActionResult Index(int id)
    {
        var articleService = HttpContext.RequestServices.GetService<ArticleService>();
        var model = articleService?.GetArticleById(id);
        if (model == null) return NotFound();
        var postVm = new PostVm
        {
            Article = model.Body,
            HeaderExt = model.Header,
            BodyExt = model.Footer,
            Relevant = articleService.GetRelevantArticle(model.Body),
            PrevArticle = articleService.GetPrevArticle(id),
            NextArticle = articleService.GetNextArticle(id),
            CommentCount = model.CommentCount
        };
        Request.Cookies.TryGetValue(nameof(CommentEntity.AuthorName), out var nikeName);
        Request.Cookies.TryGetValue(nameof(CommentEntity.AuthorEmail), out var email);
        Request.Cookies.TryGetValue(nameof(CommentEntity.AuthorUrl), out var url);
        ViewData[nameof(CommentEntity.AuthorName)] = nikeName;
        ViewData[nameof(CommentEntity.AuthorEmail)] = email;
        ViewData[nameof(CommentEntity.AuthorUrl)] = url;
        ViewData[nameof(CommentEntity.ArticleId)] = id;
        return View(postVm);
    }
}