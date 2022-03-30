using System.Collections.Generic;
using Jx.Cms.Plugin.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Plugin.Plugin
{
    /// <summary>
    /// 后台相关接口
    /// </summary>
    public interface ISystemPlugin
    {
        /// <summary>
        /// 插件被启用时
        /// </summary>
        void PluginEnable()
        {
        }

        /// <summary>
        /// 添加后台菜单项
        /// </summary>
        /// <returns></returns>
        List<PluginMenuModel> AddMenuItem()
        {
            return null;
        }

        /// <summary>
        /// 添加中间件
        /// </summary>
        /// <param name="context"></param>
        /// <returns>停止中间件执行返回false，否则返回true</returns>
        bool AddMiddleware(HttpContext context)
        {
            return true;
        }

        /// <summary>
        /// 插件被禁用时
        /// </summary>
        void PluginDisable()
        {
        }

        /// <summary>
        /// 插件被删除时
        /// </summary>
        void PluginDeleted()
        {
            
        }
        
    }
}