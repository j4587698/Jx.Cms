using System;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Plugin.Model
{
    /// <summary>
    /// 插件添加菜单
    /// </summary>
    public class PluginMenuModel
    {
        /// <summary>
        /// 菜单Id
        /// </summary>
        public string MenuId { get; set; }
        
        /// <summary>
        /// 父菜单Id，目前只支持放入系统菜单中
        /// </summary>
        public string ParentId { get; set; } 

        /// <summary>
        /// 菜单展示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 插件的界面
        /// </summary>
        public RenderFragment PluginBody { get; set; }
    }
}