using Blogs.Components;
using BootstrapBlazor.Components;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Microsoft.AspNetCore.Http;

namespace Blogs
{
    public class RewriterInstance: ISystemPlugin
    {
        public PluginMenuModel AddMenuItem()
        {
            return new PluginMenuModel()
            {
                DisplayName = "Blogs主题伪静态设置",
                Icon = "fa fa-code",
                MenuId = "AB0050BE-9EC0-49B8-A2F6-0403880D9B23",
                PluginBody = BootstrapDynamicComponent.CreateComponent<RewriteSettings>().Render()
            };
        }

        public bool AddMiddleware(HttpContext context)
        {
            
            return true;
        }
    }
}