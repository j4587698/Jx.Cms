using System.Collections.Generic;
using System.ComponentModel;
using FreeSql;

namespace Jx.Cms.Entities.Admin
{
    [Description("分类信息")]
    public class CatalogueEntity:BaseEntity<CatalogueEntity, int>
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [Description("分类名称")]
        public string Name { get; set; }

        /// <summary>
        /// 父分类ID
        /// </summary>
        [Description("父分类ID")]
        public int ParentId { get; set; }

        /// <summary>
        /// 父分类
        /// </summary>
        [Description("父分类")]
        public CatalogueEntity Parent { get; set; }

        /// <summary>
        /// 子分类列表
        /// </summary>
        [Description("子分类列表")]
        public ICollection<CatalogueEntity> Children { get; set; }
    }
}