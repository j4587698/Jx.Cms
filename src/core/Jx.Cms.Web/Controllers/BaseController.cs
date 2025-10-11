using System.Security.Claims;
using Jx.Cms.Plugin.Service.Admin;
using Jx.Cms.Plugin.Service.Both;
using Jx.Cms.Themes.Vm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Jx.Cms.Web.Controllers;

public abstract class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        var settings = SystemSettingsVm.Init();

        ViewData["settings"] = settings;


        ViewData["menu"] = HttpContext.RequestServices.GetService<IMenuService>()?.GetAllMenuTree();

        if (User != null && User.Identity.IsAuthenticated)
        {
            var username = User.FindFirst(ClaimTypes.Name);
            if (username != null)
                ViewData["user"] = HttpContext.RequestServices.GetService<IAdminUserService>()
                    ?.GetUserByUserName(username.Value);
        }
    }
}