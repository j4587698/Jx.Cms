using System;

namespace Jx.Cms.Admin.Attribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MenuAttribute:System.Attribute
    {
        /// <summary>
        /// 菜单Id（必须唯一）
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 显示名字
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 显示顺序，从大到小
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 图标类
        /// </summary>
        public string IconClass { get; set; }

        /// <summary>
        /// 上级菜单名称
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }


        public MenuAttribute(string id, string displayName, string path, int order = 0, string iconClass = "",
            string parentId = "")
        {
            Id = id;
            DisplayName = displayName;
            Path = path;
            Order = order;
            IconClass = iconClass;
            ParentId = parentId;
        }
    }
}