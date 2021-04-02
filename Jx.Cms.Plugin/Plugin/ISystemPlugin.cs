using Jx.Cms.Plugin.Model;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Plugin.Plugin
{
    /// <summary>
    /// 后台相关接口
    /// </summary>
    public interface ISystemPlugin
    {
        PluginMenuModel AddMenuItem()
        {
            return null;
        }
        
    }
}