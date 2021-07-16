using System.Collections.Generic;
using Furion;
using Jx.Cms.Entities.Admin;
using Jx.Cms.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Service.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IArticleService = Jx.Cms.Service.Front.IArticleService;

namespace Blogs.Pages.Blogs
{
    public class Article : PageModel
    {
        public ArticleModel _article { get; set; }

        public ArticleEntity _prev { get; set; }

        public ArticleEntity _next { get; set; }
        
        public AdminUserEntity _AdminUser { get; set; }

        public List<ArticleEntity> Relevant { get; set; }
        
        public IActionResult OnGet([FromQuery]int id, [FromServices]IArticleService articleService, [FromServices]IAdminUserService adminUserService)
        {
            _article = articleService.GetArticleById(id);
            if (_article == null)
            {
                return RedirectToPage("/Index");
            }
            _prev = articleService.GetPrevArticle(id);
            _next = articleService.GetNextArticle(id);
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                _AdminUser = adminUserService.GetUserByUserName(HttpContext.User.Identity.Name);
            }

            Relevant = articleService.GetRelevantArticle(_article.Body, 8);
            return Page();
        }
    }
}