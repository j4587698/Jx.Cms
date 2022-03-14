using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Jx.Cms.Web.Filter;

public class SettingsFilterAttribute : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }
}