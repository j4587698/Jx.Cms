﻿using System.Security.Claims;
using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.Service.Admin;
using Jx.Cms.Service.Both;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Jx.Cms.Web.Controllers;

public abstract class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        var settings = App.GetService<ISettingsService>().GetValuesByNames(SettingsConstants.GetAllSystemKey());
        foreach (var setting in settings)
        {
            ViewData[setting.Key] = setting.Value;
        }

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