using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FreeSql;

namespace Jx.Cms.Entities.Article
{
    /// <summary>
    /// 分类目录
    /// </summary>
    [Description("分类目录")]
    public class CatalogEntity : BaseEntity<CatalogEntity, int>
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [Description("分类名称")]
        [Required(ErrorMessage = "分类名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 分类标题
        /// </summary>
        [Description("分类别名")]
        public string Alias { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        [Description("父ID")]
        public int ParentId { get; set; }

        /// <summary>
        /// 父分类
        /// </summary>
        [Description("父分类")]
        public virtual CatalogEntity Parent { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string Description { get; set; }

        /// <summary>
        /// 子目录
        /// </summary>
        [Description("子分类")]
        public virtual ICollection<CatalogEntity> Children { get; set; }
    }
}