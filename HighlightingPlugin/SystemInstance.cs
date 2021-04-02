using BootstrapBlazor.Components;
using HighlightingPlugin.Pages;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Microsoft.AspNetCore.Components;

namespace HighlightingPlugin
{
    public class SystemInstance: ISystemPlugin
    {
        public PluginMenuModel AddMenuItem()
        {
            return new PluginMenuModel()
            {
                DisplayName = "代码高亮设置",
                Icon = "fa fa-code",
                MenuId = "77069BFC-4B62-43E5-8021-B06933D18630",
                PluginBody = BootstrapDynamicComponent.CreateComponent<HighlightingDialog>().Render()
            };
        }
    }
}