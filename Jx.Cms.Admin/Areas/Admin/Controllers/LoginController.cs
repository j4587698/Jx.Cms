using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Service.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly IAdminUserService _adminUserService;

        public LoginController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }
        
        // GET
        [HttpGet]
        public IActionResult Index(string redirect)
        {
            ViewData["redirect"] = redirect;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password, bool rememberme, string redirect)
        {
            if (username.IsNullOrEmpty())
            {
                ViewData["redirect"] = redirect;
                ViewData["Error"] = "用户名不能为空";
                return View();
            }
            if (password.IsNullOrEmpty())
            {
                ViewData["redirect"] = redirect;
                ViewData["Error"] = "密码不能为空";
                return View();
            }

            var entity = _adminUserService.Login(username, password);
            if (entity != null)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, entity.UserName));
                await HttpContext.SignInAsync(new ClaimsPrincipal(identity), new AuthenticationProperties(){IsPersistent = true, ExpiresUtc = rememberme? DateTimeOffset.Now.AddDays(5): DateTimeOffset.Now.AddMinutes(30)});
                if (redirect.IsNullOrEmpty())
                {
                    return Redirect("/Admin");
                }

                return Redirect(redirect);
            }
            ViewData["redirect"] = redirect;
            ViewData["Error"] = "登录失败，请检查输入的信息";
            return View();
        }
    }
}