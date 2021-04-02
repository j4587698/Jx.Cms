using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Jx.Cms.Plugin.Components;
using Microsoft.AspNetCore.Components;

namespace HighlightingPlugin.Pages
{
    public partial class HighlightingDialog: IPluginDialog
    {
        public Task OnClose(DialogResult result)
        {
            if (result == DialogResult.Yes)
            {
                
            }
            return Task.CompletedTask;
        }
    }
}