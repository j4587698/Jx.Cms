using System;
using Furion;
using Jx.Cms.Entities.Article;
using Jx.Cms.Service.Front.Impl;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Pages.Default
{
    public class Post : DefaultPageModel
    {
        public ArticleEntity Article { get; set; }

        public ArticleEntity PrevArticle { get; set; }

        public ArticleEntity NextArticle { get; set; }
        
        public IActionResult OnGet(int id)
        {
            var articleService = App.GetService<ArticleService>();
            Console.WriteLine(id);
            Article = articleService.GetArticleById(id);
            if (Article == null)
            {
                return RedirectToPage("/Index");
            }
            PrevArticle = articleService.GetPrevArticle(Article.Id);
            NextArticle = articleService.GetNextArticle(Article.Id);
            return Page();
        }
    }
}