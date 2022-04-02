using System;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Jx.Cms.Plugin.Components;

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
        
        public Func<ResultDialogOption, Type> OnDialogCreate { get; set; }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        public Func<(DialogResult dialogResult, IPluginDialog pluginDialog), Task<string>> OnToolbarClick { get; set; }
    }
}