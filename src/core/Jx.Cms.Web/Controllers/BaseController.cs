using System.Security.Claims;
using Furion;
using Jx.Cms.Common.Utils;
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
        

        ViewData["menu"] = App.GetService<IMenuService>().GetAllMenuTree();

        if (App.User != null)
        {
            var username = App.User.FindFirst(ClaimTypes.Name);
            if (username != null)
            {
                ViewData["user"] = App.GetService<IAdminUserService>()
                    .GetUserByUserName(username.Value);
            }
        }
    }
}