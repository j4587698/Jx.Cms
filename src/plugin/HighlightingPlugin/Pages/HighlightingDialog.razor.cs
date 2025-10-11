using BootstrapBlazor.Components;
using Jx.Cms.Plugin.Components;
using Jx.Cms.Plugin.Service.Both.Impl;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace HighlightingPlugin.Pages;

public partial class HighlightingDialog : IPluginDialog
{
    private readonly IEnumerable<SelectedItem> Codes = typeof(Code).ToSelectList();

    [Inject] public IServiceProvider Services { get; set; }

    public string SelectedValue { get; set; }

    public string CodeValue { get; set; }

    public Task OnClose(DialogResult result)
    {
        if (result == DialogResult.Yes)
        {
        }

        return Task.CompletedTask;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var settingsService = Services.GetRequiredService<SettingsService>();
        SelectedValue = settingsService.GetValue(Constant.SettingsKey, Constant.DefaultCodeType) ?? "Auto";
    }
}