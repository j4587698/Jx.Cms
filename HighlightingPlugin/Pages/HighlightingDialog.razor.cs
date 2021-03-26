using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace HighlightingPlugin.Pages
{
    public partial class HighlightingDialog: ComponentBase, IResultDialog
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