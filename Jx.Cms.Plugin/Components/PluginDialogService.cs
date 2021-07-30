using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Furion.DependencyInjection;

namespace Jx.Cms.Plugin.Components
{
    public class PluginDialogService : ITransient
    {
        public async Task<(DialogResult result, IPluginDialog dialog)> ShowModal(ResultDialogOption option, Type type, DialogService dialogService)
        {
            if (!type.IsAssignableTo(typeof(IPluginDialog)) || type.IsAbstract)
            {
                throw new Exception($"类型必须继承于{nameof(IPluginDialog)}并且不为虚类");
            }

            TaskCompletionSource<DialogResult> taskCompletionSource = new TaskCompletionSource<DialogResult>();

            IPluginDialog pluginDialog = null;
            var result = DialogResult.Close;

            option.BodyTemplate = builder =>
            {
                builder.OpenComponent(0, type);
                builder.AddMultipleAttributes(1, option.ComponentParamters);
                builder.AddComponentReferenceCapture(2, com => pluginDialog = (IPluginDialog)com);
                builder.SetKey(Guid.NewGuid());
                builder.CloseComponent();
            };
            
            option.FooterTemplate = BootstrapDynamicComponent.CreateComponent<ResultDialogFooter>(new KeyValuePair<string, object>[]
            {
                new(nameof(ResultDialogFooter.ShowCloseButton), option.ShowCloseButton),
                new(nameof(ResultDialogFooter.ButtonCloseColor), option.ButtonCloseColor),
                new(nameof(ResultDialogFooter.ButtonCloseIcon), option.ButtonCloseIcon),
                new(nameof(ResultDialogFooter.ButtonCloseText), option.ButtonCloseText),
                new(nameof(ResultDialogFooter.OnClickClose), new Func<Task>(async () => {
                    result = DialogResult.Close;
                    if(option.OnCloseAsync !=null) { await option.OnCloseAsync(); }
                })),

                new(nameof(ResultDialogFooter.ShowYesButton), option.ShowYesButton),
                new(nameof(ResultDialogFooter.ButtonYesColor), option.ButtonYesColor),
                new(nameof(ResultDialogFooter.ButtonYesIcon), option.ButtonYesIcon),
                new(nameof(ResultDialogFooter.ButtonYesText), option.ButtonYesText ?? "确定"),
                new(nameof(ResultDialogFooter.OnClickYes), new Func<Task>(async () => {
                    result = DialogResult.Yes;
                    if(option.OnCloseAsync !=null) { await option.OnCloseAsync(); }
                })),

                new(nameof(ResultDialogFooter.ShowNoButton), option.ShowNoButton),
                new(nameof(ResultDialogFooter.ButtonNoColor), option.ButtonNoColor),
                new(nameof(ResultDialogFooter.ButtonNoIcon), option.ButtonNoIcon),
                new(nameof(ResultDialogFooter.ButtonNoText), option.ButtonNoText ?? "取消"),
                new(nameof(ResultDialogFooter.OnClickNo), new Func<Task>(async () => {
                    result = DialogResult.No;
                    if(option.OnCloseAsync !=null) { await option.OnCloseAsync(); }
                }))
            }).Render();

            var closeCallback = option.OnCloseAsync;
            option.OnCloseAsync = async () =>
            {
                if (pluginDialog != null && await pluginDialog.OnClosing(result))
                {
                    await pluginDialog.OnClose(result);
                    if (closeCallback != null)
                    {
                        await closeCallback();
                    }

                    // Modal 与 ModalDialog 的 OnClose 事件陷入死循环
                    // option.OnClose -> Modal.Close -> ModalDialog.Close -> ModalDialog.OnClose -> option.OnClose
                    option.OnCloseAsync = null;
                    await option.Dialog.Close();
                    taskCompletionSource.SetResult(result);
                }
            };
            await dialogService.Show(option);
            return (await taskCompletionSource.Task, pluginDialog);
        }
    }
}