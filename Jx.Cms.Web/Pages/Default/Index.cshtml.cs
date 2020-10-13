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
            Utils.PcThemeName = "TestA";
            // var libraryPath = Path.GetFullPath(
            //     Path.Combine(Directory.GetCurrentDirectory(), "Test"));
            // var assemblyFiles = Directory.GetFiles(libraryPath, "*.dll", SearchOption.AllDirectories);
            // foreach (var assemblyFile in assemblyFiles)
            // {
            //     try
            //     {
            //         var assembly = Assembly.LoadFrom(assemblyFile);
            //         if (assemblyFile.EndsWith(".Views.dll"))
            //             partManager.ApplicationParts.Add(new 
            //                 CompiledRazorAssemblyPart(assembly));
            //         else
            //             partManager.ApplicationParts.Add(new AssemblyPart(assembly));
            //     }
            //     catch (Exception e) { }
            // }
            // MyActionDescriptorChangeProvider.Instance.HasChanged = true;
            // MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();

        }
    }
}