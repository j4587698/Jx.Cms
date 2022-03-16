using System.Collections.Generic;
using Jx.Cms.Common.Utils;

namespace Jx.Cms.Plugin.Service
{
    /// <summary>
    /// 插件相关服务
    /// </summary>
    public interface IPluginService
    {
        /// <summary>
        /// 获取中所有的插件
        /// </summary>
        /// <returns></returns>
        List<PluginConfig> GetAllPlugins();
        
        /// <summary>
        /// 根据Id获取插件信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        PluginConfig GetPluginById(int id);
        
        /// <summary>
        /// 根据插件Id获取插件信息
        /// </summary>
        /// <param name="pluginId">插件Id</param>
        /// <returns></returns>
        PluginConfig GetPluginByPluginId(string pluginId);

        /// <summary>
        /// 更改插件状态
        /// </summary>
        /// <param name="pluginId">插件Id</param>
        /// <returns>当前插件状态</returns>
        bool ChangePluginStatus(string pluginId);

        /// <summary>
        /// 更改插件状态
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>当前插件状态</returns>
        bool ChangePluginStatus(int id);
        
        /// <summary>
        /// 删除插件（物理删除文件，无法恢复）
        /// </summary>
        /// <param name="pluginId">插件Id</param>
        /// <returns>是否删除成功</returns>
        bool DeletePlugin(string pluginId);

        /// <summary>
        /// 删除插件（物理删除文件，无法恢复）
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>是否删除成功</returns>
        bool DeletePlugin(int id);
    }
}