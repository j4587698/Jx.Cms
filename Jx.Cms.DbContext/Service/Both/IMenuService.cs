using System.Collections.Generic;
using Jx.Cms.Entities.Front;

namespace Jx.Cms.DbContext.Service.Both
{
    /// <summary>
    /// 菜单相关
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// 获取全部菜单项
        /// </summary>
        /// <returns></returns>
        List<MenuEntity> GetAllMenus();

        /// <summary>
        /// 获取所有友情链接
        /// </summary>
        /// <returns></returns>
        List<MenuEntity> GetAllLinks();

        /// <summary>
        /// 获取所有的自定义菜单
        /// </summary>
        /// <returns></returns>
        List<MenuEntity> GetAllCustomMenus(string menuName);

        /// <summary>
        /// 获取指定Id下所有的菜单项
        /// </summary>
        /// <returns></returns>
        List<MenuEntity> GetMenusWithId(int id);

        /// <summary>
        /// 获取除当前Id外的所有菜单项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<MenuEntity> GetMenusWithOutId(int id);
        
        /// <summary>
        /// 获取全部菜单项并转换位树型
        /// </summary>
        /// <returns></returns>
        List<MenuEntity> GetAllMenuTree();

        /// <summary>
        /// 获取所有的友情链接并转换成树形
        /// </summary>
        /// <returns></returns>
        List<MenuEntity> GetAllLinkTree();

        /// <summary>
        /// 获取所有的自定义菜单并转换为树形
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns></returns>
        List<MenuEntity> GetAllCustomMenuTree(string menuName);

        /// <summary>
        /// 获取某菜单的子菜单
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        List<MenuEntity> GetMenuByParentId(int parentId = 0);

        /// <summary>
        /// 获取某友情连接的子链接
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        List<MenuEntity> GetLinkByParentId(int parentId = 0);

        /// <summary>
        /// 获取某自定义菜单下的所有子菜单
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        List<MenuEntity> GetCustomMenuByParentId(string menuName, int parentId = 0);

        /// <summary>
        /// 保存菜单
        /// </summary>
        /// <param name="menu">菜单内容</param>
        /// <returns></returns>
        bool SaveMenu(MenuEntity menu);

        /// <summary>
        /// 保存菜单
        /// </summary>
        /// <param name="menus">菜单列表</param>
        /// <returns></returns>
        bool SaveMenu(IEnumerable<MenuEntity> menus);

        /// <summary>
        /// 删除菜单（子菜单变为顶级菜单）
        /// </summary>
        /// <param name="menuEntity"></param>
        /// <returns></returns>
        bool DeleteMenu(MenuEntity menuEntity);

        /// <summary>
        /// 删除菜单（子菜单变为顶级菜单）
        /// </summary>
        /// <param name="menuEntities"></param>
        /// <returns></returns>
        bool DeleteMenu(IEnumerable<MenuEntity> menuEntities);
    }
}