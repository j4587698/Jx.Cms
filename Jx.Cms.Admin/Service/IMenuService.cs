using System.Collections.Generic;
using BootstrapBlazor.Components;

namespace Jx.Cms.Admin.Service
{
    public interface IMenuService
    {
        List<MenuItem> GetAllMenu();
    }
}