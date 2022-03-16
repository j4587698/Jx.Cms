using System.Collections.Generic;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Furion;
using Jx.Cms.DbContext.Service.Both.Impl;
using Jx.Cms.Plugin.Components;
using Microsoft.AspNetCore.Components;

namespace HighlightingPlugin.Pages
{
    public partial class HighlightingDialog: IPluginDialog
    {
        
        private readonly IEnumerable<SelectedItem> Codes = typeof(Code).ToSelectList();

        public string SelectedValue { get; set; }

        public string CodeValue { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var settingsService = App.GetService<SettingsService>();
            SelectedValue = settingsService.GetValue(Constant.SettingsKey, Constant.DefaultCodeType) ?? "Auto";
        }

        public Task OnClose(DialogResult result)
        {
            if (result == DialogResult.Yes)
            {
                
            }
            return Task.CompletedTask;
        }
    }
}