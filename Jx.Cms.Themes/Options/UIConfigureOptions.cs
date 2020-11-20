﻿using System;
using System.IO;
using Jx.Cms.Plugin;
using Jx.Cms.Themes.FileProvider;
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
            Utils.ThemeModify = ChangeTheme;
            ChangeTheme(Utils.GetThemeName());
        }

        private readonly MyCompositeFileProvider _filesProvider = new MyCompositeFileProvider();
        private string basePath = "wwwroot";

        public void ChangeTheme(string themeName)
        {
            var dllName = Utils.PathDllDic[Utils.ThemePathDic[themeName]];
            var assembly = RazorPlugin.GetAssemblyByDllName(dllName);
            if (assembly != null)
            {
                _filesProvider.ModifyFileProvider(new EmbeddedFileProvider(assembly, $"{Path.GetFileNameWithoutExtension(dllName)}.{basePath}"));
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