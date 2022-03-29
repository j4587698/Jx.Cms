using BootstrapBlazor.Components;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;

namespace CnBlogAsync;

public class SystemInstance : ISystemPlugin
{
    public void PluginEnable()
    {
        
    }

    public void PluginDeleted()
    {
        
    }

    public List<PluginMenuModel> AddMenuItem()
    {
        return new List<PluginMenuModel>()
        {
            new PluginMenuModel()
            {
                DisplayName = "博客园同步设置",
                Icon = "fa fa-home",
                MenuId = "CE9444D4-B23A-47D2-9F30-76B9599D92AB",
                PluginBody = BootstrapDynamicComponent.CreateComponent<Settings>().Render()
            }
        };
    }
}