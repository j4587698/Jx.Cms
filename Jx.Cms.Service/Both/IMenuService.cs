using System.Collections.Generic;
using Jx.Cms.Entities.Front;

namespace Jx.Cms.Service.Both.Impl
{
    public interface IMenuService
    {
        List<MenuEntity> GetAllMenuTree();

        bool SaveMenu(MenuEntity menu);
    }
}