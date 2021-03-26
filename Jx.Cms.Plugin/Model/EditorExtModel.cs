using System;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Plugin.Model
{
    /// <summary>
    /// 编辑器扩展类
    /// </summary>
    public class EditorExtModel
    {
        /// <summary>
        /// 按钮信息
        /// </summary>
        public EditorToolbarButton ToolbarButton { get; set; }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        public Func<string, Task<string>> OnToolbarClick { get; set; }
    }
}