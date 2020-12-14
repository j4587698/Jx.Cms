using System;
using System.IO;
using System.Reflection;
using Jx.Cms.Themes;
using Jx.Cms.Themes.Options;
using Jx.Cms.Themes.PartManager;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Jx.Cms.Web.Pages.Default
{
    public class Index : PageModel
    {
        private ApplicationPartManager partManager;
        public Index(ApplicationPartManager app)
        {
            partManager = app;
            //_options = options;
        }
        public void OnGet()
        {
            //Utils.SetTheme("TestA", Utils.ThemeMode.PcTheme);

        }
    }
}