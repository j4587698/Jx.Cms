using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Furion.DynamicApiController;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Plugin.Service.Admin;
using Jx.Cms.Web.Vo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Admin.Controllers
{
    [Route("api/Admin/User")]
    public class UserController : IDynamicApiController
    {
        private readonly IAdminUserService _adminUserService;

        public UserController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpPost]
        public async Task<object> Login([FromBody]LoginVo loginVo)
        {
            if (loginVo.UserName.IsNullOrEmpty())
            {
                return new { code = 50000, message = "用户名不能为空" };
            }
            if (loginVo.Password.IsNullOrEmpty())
            {
                return new { code = 50000, message = "密码不能为空" };
            }

            var entity = _adminUserService.Login(loginVo.UserName, loginVo.Password);
            if (entity != null)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, entity.UserName));
                await Furion.App.HttpContext.SignInAsync(new ClaimsPrincipal(identity), new AuthenticationProperties(){IsPersistent = true, ExpiresUtc = loginVo.RememberMe? DateTimeOffset.Now.AddDays(5): DateTimeOffset.Now.AddMinutes(30)});

                return new { code = 20000, message = "登录成功" };
            }
            return new { code = 50000, message = "用户名或密码错误" };
        }
    }
}