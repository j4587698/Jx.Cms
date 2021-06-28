using Jx.Cms.Entities.Article;
using Jx.Cms.Service.Front;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blogs.Pages.Blogs
{
    public class Article : PageModel
    {
        public ArticleEntity _article { get; set; }

        public ArticleEntity Prev { get; set; }

        public ArticleEntity Next { get; set; }
        
        public void OnGet([FromQuery]int id, [FromServices]IArticleService articleService)
        {
            _article = ArticleEntity.Find(id);
            Prev = articleService.GetPrevArticle(id);
            Next = articleService.GetNextArticle(id);
        }
    }
}