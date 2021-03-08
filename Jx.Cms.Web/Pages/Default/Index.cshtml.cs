﻿using System.Collections.Generic;
using Furion;
using Jx.Cms.Entities.Article;
using Jx.Cms.Service.Front;

namespace Jx.Cms.Web.Pages.Default
{
    public class Index : DefaultPageModel
    {
        public long Count { get; set; }

        public int PageNo { get; set; }

        public long TotalPage { get; set; }

        public int StartNo { get; set; }

        public IEnumerable<ArticleEntity> Articles { get; set; }

        private int pageSize = 1;
        
        public void OnGet(int pageNo)
        {
            if (pageNo == 0)
            {
                pageNo = 1;
            }
            var articleService = App.GetService<IArticleService>();
            Articles = articleService.GetArticlePageWithCount(pageNo, pageSize, out var count);
            Count = count;
            PageNo = pageNo;
            TotalPage = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
            if (TotalPage < 5)
            {
                StartNo = 1;
            }
            else if (pageNo + 2 > TotalPage)
            {
                StartNo = (int)TotalPage - 4;
            }
            else if (pageNo - 2 < 1)
            {
                StartNo = 1;
            }
            else
            {
                StartNo = pageNo - 2;
            }
        }
    }
}