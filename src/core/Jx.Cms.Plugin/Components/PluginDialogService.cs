using BootstrapBlazor.Components;

namespace Jx.Cms.Plugin.Components;

public class PluginDialogService
{
    public async Task<(DialogResult result, IPluginDialog dialog)> ShowModal(ResultDialogOption option, Type type,
        DialogService dialogService)
    {
        if (!type.IsAssignableTo(typeof(IPluginDialog)) || type.IsAbstract)
            throw new Exception($"类型必须继承于{nameof(IPluginDialog)}并且不为虚类");

        var taskCompletionSource = new TaskCompletionSource<DialogResult>();

        IPluginDialog pluginDialog = null;
        var result = DialogResult.Close;
        option.Size = Size.ExtraLarge;
        option.BodyTemplate = builder =>
        {
            builder.OpenComponent(0, type);
            builder.AddMultipleAttributes(1, option.ComponentParameters);
            builder.AddComponentReferenceCapture(2, com => pluginDialog = (IPluginDialog)com);
            builder.SetKey(Guid.NewGuid());
            builder.CloseComponent();
        };

        option.FooterTemplate = BootstrapDynamicComponent.CreateComponent<ResultDialogFooter>(
            new Dictionary<string, object>
            {
                { nameof(ResultDialogFooter.ShowCloseButton), option.ShowCloseButton },
                { nameof(ResultDialogFooter.ButtonCloseColor), option.ButtonCloseColor },
                { nameof(ResultDialogFooter.ButtonCloseIcon), option.ButtonCloseIcon },
                { nameof(ResultDialogFooter.ButtonCloseText), option.ButtonCloseText },
                {
                    nameof(ResultDialogFooter.OnClickClose), new Func<Task>(async () =>
                    {
                        result = DialogResult.Close;
                        if (option.OnCloseAsync != null) await option.OnCloseAsync();
                    })
                },

                { nameof(ResultDialogFooter.ShowYesButton), option.ShowYesButton },
                { nameof(ResultDialogFooter.ButtonYesColor), option.ButtonYesColor },
                { nameof(ResultDialogFooter.ButtonYesIcon), option.ButtonYesIcon },
                { nameof(ResultDialogFooter.ButtonYesText), option.ButtonYesText ?? "确定" },
                {
                    nameof(ResultDialogFooter.OnClickYes), new Func<Task>(async () =>
                    {
                        result = DialogResult.Yes;
                        if (option.OnCloseAsync != null) await option.OnCloseAsync();
                    })
                },

                { nameof(ResultDialogFooter.ShowNoButton), option.ShowNoButton },
                { nameof(ResultDialogFooter.ButtonNoColor), option.ButtonNoColor },
                { nameof(ResultDialogFooter.ButtonNoIcon), option.ButtonNoIcon },
                { nameof(ResultDialogFooter.ButtonNoText), option.ButtonNoText ?? "取消" },
                {
                    nameof(ResultDialogFooter.OnClickNo), new Func<Task>(async () =>
                    {
                        result = DialogResult.No;
                        if (option.OnCloseAsync != null) await option.OnCloseAsync();
                    })
                }
            }).Render();

        var closeCallback = option.OnCloseAsync;
        option.OnCloseAsync = async () =>
        {
            if (pluginDialog != null && await pluginDialog.OnClosing(result))
            {
                await pluginDialog.OnClose(result);
                if (closeCallback != null) await closeCallback();

                // Modal 与 ModalDialog 的 OnClose 事件陷入死循环
                // option.OnClose -> Modal.Close -> ModalDialog.Close -> ModalDialog.OnClose -> option.OnClose
                option.OnCloseAsync = null;
                // Close method not available in new API, just set result
                taskCompletionSource.TrySetResult(result);
            }
        };
        await dialogService.Show(option);
        return (await taskCompletionSource.Task, pluginDialog);
    }
}