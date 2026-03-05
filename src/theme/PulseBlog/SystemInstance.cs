using BootstrapBlazor.Components;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using PulseBlog.Components;
using PulseBlog.Utils;

namespace PulseBlog;

public class SystemInstance : ISystemPlugin
{
    public void PluginEnable()
    {
        ThemeSettings.SaveSettings(ThemeSettings.GetSettings());
    }

    public List<PluginMenuModel> AddMenuItem()
    {
        return new List<PluginMenuModel>
        {
            new()
            {
                DisplayName = "Pulse主题设置",
                Icon = "fa fa-palette",
                MenuId = "AE6F099D-BFC4-4BDA-81F6-A31C6D343A57",
                PluginBody = BootstrapDynamicComponent.CreateComponent<Settings>().Render()
            }
        };
    }
}
