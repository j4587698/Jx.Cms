using System.Collections.Generic;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Plugin.Components;
using Jx.Cms.Plugin.Service.Both.Impl;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace HighlightingPlugin.Pages
{
    public partial class HighlightingDialog: IPluginDialog
    {
        [Inject]
        public IServiceProvider Services { get; set; }
        
        private readonly IEnumerable<SelectedItem> Codes = typeof(Code).ToSelectList();

        public string SelectedValue { get; set; }

        public string CodeValue { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var settingsService = Services.GetRequiredService<SettingsService>();
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