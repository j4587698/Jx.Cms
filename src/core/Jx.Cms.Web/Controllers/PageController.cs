using Furion;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Service.Front;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Controllers;

public class PageController : BaseController
{
    public IActionResult Index(int id)
    {
        var articleService = App.GetService<IPageService>();
        var model = articleService.GetPageById(id);
        if (model == null)
        {
            return new NotFoundResult();
        }
        var pageVm = new PageVm()
        {
            Article = model.Body,
            HeaderExt = model.Header,
            BodyExt = model.Footer,
        };
        Request.Cookies.TryGetValue(nameof(CommentEntity.AuthorName), out var nikeName);
        Request.Cookies.TryGetValue(nameof(CommentEntity.AuthorEmail), out var email);
        Request.Cookies.TryGetValue(nameof(CommentEntity.AuthorUrl), out var url);
        ViewData[nameof(CommentEntity.AuthorName)] = nikeName;
        ViewData[nameof(CommentEntity.AuthorEmail)] = email;
        ViewData[nameof(CommentEntity.AuthorUrl)] = url;
        ViewData[nameof(CommentEntity.ArticleId)] = id;
        return View(pageVm);
    }
}