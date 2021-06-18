using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blogs.Pages.Blogs
{
    public class Article : PageModel
    {
        public int Id { get; set; }
        
        public void OnGet(int id)
        {
            Id = id;
        }
    }
}