using System;
using System.Collections.Generic;
using System.ComponentModel;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Admin.ViewModel
{
    /// <summary>
    /// 文章VM
    /// </summary>
    public class ArticleVm
    {
        public int Id { get; set; }
        
        [Description("文章标题")]
        public string Title { get; set; }

        [Description("别名")]
        public string Alias { get; set; }

        [Description("描述")]
        public string Description { get; set; }

        [Description("文章作者")]
        public string Author { get; set; }
        
        [Description("分类ID")]
        public int CatalogueId { get; set; }
        
        [Description("分类")]
        public CatalogEntity Catalogue { get; set; }
        
        [Description("文章内容")]
        public string Content { get; set; }

        [Description("标签")]
        public string Labels { get; set; }
        
        [Description("阅读量")]
        public int ReadingVolume { get; set; }
        
        [Description("发布时间")]
        public DateTime PublishTime { get; set; }
        
        [Description("发布状态：1.公开 2.草稿 3.待审核")]
        public ArticleStatusEnum Status { get; set; }

        [Description("是否为Markdown")]
        public bool IsMarkdown { get; set; }
    }
}