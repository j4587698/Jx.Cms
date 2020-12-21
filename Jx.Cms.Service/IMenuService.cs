using System.Collections.Generic;
using Jx.Cms.Common.Attribute;

namespace Jx.Cms.Service
{
    /// <summary>
    /// 菜单相关
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns>菜单列表</returns>
        List<MenuAttribute> GetAllMenu();
    }
}