﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FreeSql;
using FreeSql.DataAnnotations;

namespace Jx.Cms.Entities.Admin
{
    [Description("文章列表")]
    public class ArticleEntity: BaseEntity<ArticleEntity, int>
    {
        [Description("文章标题")]
        [DisplayName("标题")]
        public string Title { get; set; }

        [Description("文章作者")]
        public string Author { get; set; }
        
        [Description("分类ID")]
        public int CatalogueId { get; set; }
        
        [Description("分类")]
        [Navigate(nameof(CatalogueId))]
        public CatalogueEntity Catalogue { get; set; }
        
        [Description("文章内容")]
        [MaxLength(-1)]
        public string Content { get; set; }
        
        [Description("阅读量")]
        public int ReadingVolume { get; set; }
        
        [Description("发布时间")]
        public DateTime PublishTime { get; set; }
        
        [Description("发布状态：1.公开 2.草稿 3.待审核")]
        public int Status { get; set; }

        [Description("是否为Markdown")]
        public bool IsMarkdown { get; set; }

        [Description("Markdown内容")]
        [MaxLength(-1)]
        public string MarkdownContent { get; set; }
    }
}