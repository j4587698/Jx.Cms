using System.Collections.Generic;
using Jx.Cms.Common.Attribute;

namespace Jx.Cms.Plugin.Service.Admin
{
    /// <summary>
    /// 菜单相关
    /// </summary>
    public interface IAdminMenuService
    {
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns>菜单列表</returns>
        List<MenuAttribute> GetAllMenu();
    }
}