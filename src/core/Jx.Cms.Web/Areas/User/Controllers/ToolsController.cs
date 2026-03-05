using System.ComponentModel.DataAnnotations;
using Jx.Cms.Common.Enum;
using Jx.Cms.Common.Vo;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Service.Both;
using Jx.Cms.Themes.Vm;
using Jx.Toolbox.HtmlTools;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Areas.User.Controllers;

/// <summary>
///     工具类
/// </summary>
[Area("User")]
[Route("/User/[action]")]
public class ToolsController : ControllerBase
{
    /// <summary>
    ///     提交评论
    /// </summary>
    /// <param name="comment"></param>
    /// <param name="commentService"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public object Comment([FromForm] CommentVo comment, [FromServices] ICommentService commentService)
    {
        var systemSettingsVm = SystemSettingsVm.Init();
        if (!systemSettingsVm.CanComment) return R.Fail(500002, "评论已关闭");

        comment.AuthorName = comment.AuthorName?.Trim();
        comment.AuthorEmail = comment.AuthorEmail?.Trim();
        comment.AuthorUrl = comment.AuthorUrl?.Trim();
        comment.Content = comment.Content?.Trim();

        var validationContext = new ValidationContext(comment);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(comment, validationContext, validationResults, true);
        if (!isValid)
            return R.Fail(500002, string.Join(",", validationResults.Select(x => x.ErrorMessage)));

        if (!IsValidAuthorUrl(comment.AuthorUrl)) return R.Fail(500002, "评论者网址格式不正确");

        var article = ArticleEntity.Select
            .Where(x => x.Id == comment.ArticleId && x.Status == ArticleStatusEnum.Published)
            .First();
        if (article == null) return R.Fail(404, "文章不存在");
        if (!article.CanComment) return R.Fail(500002, "当前内容不允许评论");

        if (comment.ParentId < 0 || comment.RootId < 0) return R.Fail(500002, "评论参数错误");

        if (comment.ParentId > 0)
        {
            var parentComment = CommentEntity.Select.Where(x => x.Id == comment.ParentId).First();
            if (parentComment == null) return R.Fail(404, "父评论不存在");
            if (parentComment.ArticleId != comment.ArticleId) return R.Fail(500002, "父评论与当前文章不匹配");
            if (parentComment.Status != CommentStatusEnum.Pass) return R.Fail(500002, "父评论不可回复");
            comment.RootId = parentComment.RootId > 0 ? parentComment.RootId : parentComment.Id;
        }
        else
        {
            comment.RootId = 0;
        }

        comment.AuthorIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        comment.AuthorAgent = Request.Headers["User-Agent"].ToString();
        comment.Content = Html.RemoveHtmlTag(comment.Content);
        if (string.IsNullOrWhiteSpace(comment.Content)) return R.Fail(500002, "评论内容不能为空");
        var commentEntity = comment.Adapt<CommentEntity>();
        commentEntity.Status =
            systemSettingsVm.CommentNeedVerify ? CommentStatusEnum.NeedCheck : CommentStatusEnum.Pass;
        if (!commentService.AddOrModifyComment(commentEntity)) return R.Fail(50001, "添加评论失败");
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = Request.IsHttps,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        };
        Response.Cookies.Append(nameof(CommentEntity.AuthorName), comment.AuthorName ?? string.Empty, cookieOptions);
        Response.Cookies.Append(nameof(CommentEntity.AuthorEmail), comment.AuthorEmail ?? string.Empty, cookieOptions);
        Response.Cookies.Append(nameof(CommentEntity.AuthorUrl), comment.AuthorUrl ?? string.Empty, cookieOptions);
        return R.Success();
    }

    private static bool IsValidAuthorUrl(string authorUrl)
    {
        if (string.IsNullOrWhiteSpace(authorUrl)) return true;
        if (!Uri.TryCreate(authorUrl, UriKind.Absolute, out var parsedUri)) return false;
        return parsedUri.Scheme == Uri.UriSchemeHttp || parsedUri.Scheme == Uri.UriSchemeHttps;
    }
}
