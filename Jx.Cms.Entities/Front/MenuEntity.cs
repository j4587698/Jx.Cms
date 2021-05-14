using System.Collections.Generic;
using System.ComponentModel;
using FreeSql;
using FreeSql.DataAnnotations;
using Jx.Cms.Common.Enum;

namespace Jx.Cms.Entities.Front
{
    [Description("菜单")]
    public class MenuEntity: BaseEntity<MenuEntity, int>
    {
        [Description("菜单名")]
        public string Name { get; set; }

        [Description("菜单类型")]
        public MenuTypeEnum MenuType { get; set; }

        [Description("菜单地址")]
        public string Url { get; set; }

        [Description("类型的Id")]
        public int TypeId { get; set; }

        [Description("导航标签")]
        public string NavTitle { get; set; }

        [Description("是否新窗打开")]
        public bool OpenInNewWindow { get; set; }

        [Description("链接描述")]
        public string Title { get; set; }

        [Description("父ID")]
        public int ParentId { get; set; }

        [Navigate(nameof(ParentId))]
        public MenuEntity Parent { get; set; }

        [Navigate(nameof(ParentId))]
        public List<MenuEntity> Children { get; set; }
        
        [Column(IsIgnore = true)]
        public bool HasChildren => Children is {Count: > 0};
    }
}