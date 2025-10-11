using System.Security.Claims;
using Jx.Cms.Plugin.Service.Admin;
using Jx.Cms.Web.Vo;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Admin.Controllers;

[Route("api/Admin/[controller]/[action]")]
[ApiController]
public class UserController(IAdminUserService adminUserService) : ControllerBase
{
    public async Task<object> Login([FromBody] LoginVo loginVo)
    {
        if (loginVo.UserName.IsNullOrEmpty()) return new { code = 50000, message = "用户名不能为空" };
        if (loginVo.Password.IsNullOrEmpty()) return new { code = 50000, message = "密码不能为空" };

        var entity = adminUserService.Login(loginVo.UserName, loginVo.Password);
        if (entity != null)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, entity.UserName));
            await HttpContext.SignInAsync(new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = loginVo.RememberMe ? DateTimeOffset.Now.AddDays(5) : DateTimeOffset.Now.AddMinutes(30)
                });

            return new { code = 20000, message = "登录成功" };
        }

        return new { code = 50000, message = "用户名或密码错误" };
    }
}