using Jx.Cms.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Service.Front;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blogs.Pages.Blogs
{
    public class Article : PageModel
    {
        public ArticleModel _article { get; set; }

        public ArticleEntity _prev { get; set; }

        public ArticleEntity _next { get; set; }
        
        public IActionResult OnGet([FromQuery]int id, [FromServices]IArticleService articleService)
        {
            _article = articleService.GetArticleById(id);
            if (_article == null)
            {
                return RedirectToPage("/Index");
            }
            _prev = articleService.GetPrevArticle(id);
            _next = articleService.GetNextArticle(id);
            return Page();
        }
    }
}