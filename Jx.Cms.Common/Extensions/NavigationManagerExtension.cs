using System;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Common.Extensions
{
    public static class NavigationManagerExtension
    {

        public static Uri GetAbsoluteUri(this NavigationManager navigation)
        {
            return navigation.ToAbsoluteUri(navigation.Uri);
        }

        public static string GetLocalPath(this NavigationManager navigation)
        {
            return GetAbsoluteUri(navigation).LocalPath;
        }

        public static string QueryString(this NavigationManager navigation, string paramName)
        {
            var uri = GetAbsoluteUri(navigation);
            return HttpUtility.ParseQueryString(uri.Query).Get(paramName) ?? "";
        }
    }
}