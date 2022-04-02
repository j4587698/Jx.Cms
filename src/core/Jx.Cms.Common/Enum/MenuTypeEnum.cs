using System.ComponentModel;

namespace Jx.Cms.Common.Enum
{
    /// <summary>
    /// 菜单项类型
    /// </summary>
    public enum MenuTypeEnum
    {
        [Description("页面")]
        Page,
        [Description("文章页")]
        Article,
        [Description("自定义链接")]
        CustomUrl,
        [Description("分类目录")]
        Catalogue
    }
}