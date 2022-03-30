using System.Collections.Generic;
using System.Linq;
using FreeSql;
using Furion.DependencyInjection;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext.Entities.Front;

namespace Jx.Cms.DbContext.Service.Both.Impl
{
    public class MenuService: IMenuService, ITransient
    {

        public List<MenuEntity> GetAllMenus()
        {
            return GetAllCustomMenus(Constants.Menu);
        }

        public List<MenuEntity> GetAllLinks()
        {
            return GetAllCustomMenus(Constants.Link);
        }

        public List<MenuEntity> GetAllCustomMenus(string menuName)
        {
            return MenuEntity.Where(x => x.MenuName == menuName).OrderByDescending(x => x.Order).ToList();
        }

        public List<MenuEntity> GetMenusWithId(int id)
        {
            return MenuEntity.Where(x => x.Id == id).AsTreeCte().OrderByDescending(x => x.Order).ToList();
        }

        public List<MenuEntity> GetMenusWithOutId(int id)
        {
            var ids = GetMenusWithId(id);
            return GetAllMenus().Where(x => !ids.Exists(y => y.Id == x.Id)).ToList();
        }

        public List<MenuEntity> GetAllMenuTree()
        {
            return GetAllCustomMenuTree(Constants.Menu);
        }

        public List<MenuEntity> GetAllLinkTree()
        {
            return GetAllCustomMenuTree(Constants.Link);
        }

        public List<MenuEntity> GetAllCustomMenuTree(string menuName)
        {
            return MenuEntity.Where(x => x.ParentId == 0 && x.MenuName == menuName).AsTreeCte().OrderByDescending(x => x.Order).ToTreeList();
        }

        public List<MenuEntity> GetMenuByParentId(int parentId = 0)
        {
            return GetCustomMenuByParentId(Constants.Menu, parentId);
        }

        public List<MenuEntity> GetLinkByParentId(int parentId = 0)
        {
            return GetCustomMenuByParentId(Constants.Link, parentId);
        }

        public List<MenuEntity> GetCustomMenuByParentId(string menuName, int parentId = 0)
        {
            return MenuEntity.Where(x => x.ParentId == parentId && x.MenuName == menuName).OrderByDescending(x => x.Order).IncludeMany(x => x.Children).ToList();
        }

        public bool SaveMenu(MenuEntity menu)
        {
            menu.Save();
            return true;
        }

        public bool SaveMenu(IEnumerable<MenuEntity> menus)
        {
            BaseEntity.Orm.Transaction(() =>
            {
                foreach (var menuEntity in menus)
                {
                    menuEntity.Save();
                }
            });
            return true;
        }

        public bool DeleteMenu(MenuEntity menuEntity)
        {
            BaseEntity.Orm.Transaction(() =>
            {
                MenuEntity.Where(x => x.ParentId == menuEntity.Id).ToUpdate().Set(x => x.ParentId, 0).ExecuteAffrows();
                menuEntity.Delete(true);
            });
            return true;
        }

        public bool DeleteMenu(IEnumerable<MenuEntity> menuEntities)
        {
            return menuEntities.All(DeleteMenu);
        }
    }
}