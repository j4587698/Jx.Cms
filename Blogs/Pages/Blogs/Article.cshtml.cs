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
        
        public AdminUserEntity AdminUserEntity { get; set; }
        
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
                AdminUserEntity = adminUserService.GetUserByUserName(HttpContext.User.Identity.Name);
            }
            return Page();
        }
    }
}