using System.ComponentModel;

namespace Jx.Cms.Common.Enum
{
    /// <summary>
    /// 菜单项类型
    /// </summary>
    public enum MenuTypeEnum
    {
        Page,
        [Description("文章页")]
        Article,
        CustomUrl,
        Catalogue
    }
}