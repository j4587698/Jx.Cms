using System.Collections.Generic;
using Blogs.Components;
using BootstrapBlazor.Components;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;

namespace Blogs
{
    public class SystemInstance: ISystemPlugin
    {
        public List<PluginMenuModel> AddMenuItem()
        {
            return new()
            {
                new PluginMenuModel()
                {
                    DisplayName = "Blogs主题设置",
                    Icon = "fa fa-code",
                    MenuId = "D07208D1-D16C-487D-A57B-1B9148A27835",
                    PluginBody = BootstrapDynamicComponent.CreateComponent<Settings>().Render()
                }
            };
        }
    }
}