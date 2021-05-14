using System.Collections.Generic;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Front;

namespace Jx.Cms.Service.Both.Impl
{
    public class MenuService: IMenuService, ITransient
    {
        public List<MenuEntity> GetAllMenuTree()
        {
            return MenuEntity.Select.ToTreeList();
        }

        public bool SaveMenu(MenuEntity menu)
        {
            menu.Save();
            return true;
        }
    }
}