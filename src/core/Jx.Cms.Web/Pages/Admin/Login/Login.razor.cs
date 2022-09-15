using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Furion.ClayObject;
using Jx.Cms.Web.Vo;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Web.Pages.Admin.Login;

public partial class Login
{
    private string Title { get; set; } = "JX.CMS";

    [SupplyParameterFromQuery]
    [Parameter]
    public string ReturnUrl { get; set; }

    private LoginVo LoginVo { get; set; } = new LoginVo();

    [Inject]
    private AjaxService AjaxService { get; set; }
    
    [Inject]
    [NotNull]
    public MessageService MessageService { get; set; }

    private Task OnSignUp()
    {
        throw new NotImplementedException();
    }

    private Task OnForgotPassword()
    {
        throw new NotImplementedException();
    }

    private async Task DoLogin()
    {
        if (LoginVo.UserName.IsNullOrEmpty())
        {
            await MessageService.Show(new MessageOption()
            {
                Color = Color.Danger,
                Content = "用户名不能为空"
            });
            return;
        }
        
        if (LoginVo.Password.IsNullOrEmpty())
        {
            await MessageService.Show(new MessageOption()
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
        var str = await AjaxService.GetMessage(ajaxOption);
        if (str.IsNullOrEmpty())
        {
            await MessageService.Show(new MessageOption()
            {
                Color = Color.Danger,
                Content = "登录失败"
            });
        }
        else
        {
            dynamic ret = Clay.Parse(str);
            if (ret.code != 20000)
            {
                await MessageService.Show(new MessageOption()
                {
                    Color = Color.Danger,
                    Content = ret.message
                });
            }
            else
            {
                await MessageService.Show(new MessageOption()
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