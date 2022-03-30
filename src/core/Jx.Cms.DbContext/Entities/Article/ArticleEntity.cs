using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FreeSql;
using FreeSql.DataAnnotations;
using Jx.Cms.Common.Enum;

namespace Jx.Cms.DbContext.Entities.Article
{
    [Description("文章")]
    public class ArticleEntity : BaseEntity<ArticleEntity, int>
    {
        [Description("文章标题")]
        [DisplayName("标题")]
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
        [Navigate(nameof(CatalogueId))]
        public CatalogEntity Catalogue { get; set; }
        
        [Description("文章内容")]
        [MaxLength(-1)]
        public string Content { get; set; }

        [Description("标签")]
        [Navigate(ManyToMany = typeof(ArticleTagEntity))]
        public List<TagEntity> Labels { get; set; }
        
        [Description("阅读量")]
        public int ReadingVolume { get; set; }
        
        [Description("发布时间")]
        public DateTime PublishTime { get; set; }
        
        [Description("发布状态：1.公开 2.草稿 3.待审核")]
        public ArticleStatusEnum Status { get; set; }

        [Description("是否为Markdown")]
        public bool IsMarkdown { get; set; }

        [Description("是否为独立页面")]
        public bool IsPage { get; set; }

        [Description("是否允许评论")] public bool CanComment { get; set; } = true;

        [Description("评论列表")]
        [Navigate(nameof(CommentEntity.ArticleId))]
        public ICollection<CommentEntity> Comments { get; set; }
        
        [Description("扩展字段")]
        [Navigate(nameof(ArticleMetaEntity.ArticleId))]
        public ICollection<ArticleMetaEntity> Metas { get; set; }
    }
}