using System.Collections.Generic;
using System.Linq;
using Jx.Cms.Themes.Util;
using Masuit.Tools;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Jx.Cms.Themes;

public class TemplateViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        context.Values["template"] = ThemeUtil.GetThemeName();
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (!context.AreaName.IsNullOrEmpty())
        {
            return viewLocations;
        }
        var themeName = context.Values["template"] ?? ThemeUtil.PcThemeName;
        string[] locations = { "/Pages/" + themeName + "/{1}/{0}.cshtml", "/Pages/" + themeName + "/{0}.cshtml", "/Pages/" + themeName + "/Shared/{0}.cshtml", "/Pages/Shared/{0}.cshtml" };
        return locations.Union(viewLocations.Where(x => !x.StartsWith("/Views/")));
    }
}