using System;
using Jx.Cms.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Jx.Cms.Themes.Options
{
    public class UiConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
        public UiConfigureOptions(IWebHostEnvironment environment)
        {
            Environment = environment;
            ChangeTheme();
        }

        private IFileProvider _filesProvider;
        private string basePath = "wwwroot";

        public void ChangeTheme()
        {
            var assembly = RazorPlugin.GetAssemblyByDllName(Utils.GetThemeName());
            if (assembly != null)
            {
                _filesProvider = new EmbeddedFileProvider(assembly, basePath);
            }
        }
        
        public IWebHostEnvironment Environment { get; }

        public void PostConfigure(string name, StaticFileOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            // Basic initialization in case the options weren't initialized by any other component
            options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();
            if (options.FileProvider == null && Environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider ??= Environment.WebRootFileProvider;
            
            options.FileProvider = new CompositeFileProvider(options.FileProvider, _filesProvider);
        }
    }
}