using System.Diagnostics.CodeAnalysis;
using BootstrapBlazor.Components;
using Jx.Cms.Web.Vo;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Web.Components.Admin.Login;

public partial class Login
{
    private string Title { get; } = "JX.CMS";

    [SupplyParameterFromQuery] [Parameter] public string ReturnUrl { get; set; }

    private LoginVo LoginVo { get; } = new();

    [Inject] private AjaxService AjaxService { get; set; }

    [Inject] [NotNull] public MessageService MessageService { get; set; }

    private Task OnSignUp()
    {
        return MessageService.Show(new MessageOption
        {
            Color = Color.Info,
            Content = "暂未开放注册，请联系管理员。"
        });
    }

    private Task OnForgotPassword()
    {
        return MessageService.Show(new MessageOption
        {
            Color = Color.Info,
            Content = "请联系管理员重置密码。"
        });
    }

    private async Task DoLogin()
    {
        if (LoginVo.UserName.IsNullOrEmpty())
        {
            await MessageService.Show(new MessageOption
            {
                Color = Color.Danger,
                Content = "用户名不能为空"
            });
            return;
        }

        if (LoginVo.Password.IsNullOrEmpty())
        {
            await MessageService.Show(new MessageOption
            {
                Color = Color.Danger,
                Content = "密码不能为空"
            });
            return;
        }

        var ajaxOption = new AjaxOption
        {
            Url = "/api/Admin/User/Login",
            Data = LoginVo
        };
        var jsonDoc = await AjaxService.InvokeAsync(ajaxOption);
        if (jsonDoc == null)
        {
            await MessageService.Show(new MessageOption
            {
                Color = Color.Danger,
                Content = "登录失败"
            });
        }
        else
        {
            var root = jsonDoc.RootElement;
            
            // 检查响应是否包含必要的属性
            if (!root.TryGetProperty("code", out var codeElement) || 
                !root.TryGetProperty("message", out var messageElement))
            {
                await MessageService.Show(new MessageOption
                {
                    Color = Color.Danger,
                    Content = "服务器响应格式错误"
                });
                return;
            }
            
            var code = codeElement.GetInt32();
            var message = messageElement.GetString();

            if (code != 20000)
            {
                await MessageService.Show(new MessageOption
                {
                    Color = Color.Danger,
                    Content = message
                });
            }
            else
            {
                await MessageService.Show(new MessageOption
                {
                    Color = Color.Success,
                    Content = "登录成功"
                });
                ReturnUrl ??= "/Admin";
                await AjaxService.Goto(ReturnUrl);
            }
        }
    }
}
