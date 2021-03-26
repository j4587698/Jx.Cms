using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.FileProvider;
using Jx.Cms.Plugin.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Jx.Cms.Plugin.Options
{
    public class UiConfigureOptions: IPostConfigureOptions<StaticFileOptions>
    {

        private MyCompositeFileProvider _filesProvider;
        private string _basePath = "wwwroot";
        
        public void ModifyPlugin(PluginConfig pluginConfig)
        {
            _filesProvider.ModifyPlugin(pluginConfig, new EmbeddedFileProvider(DefaultPlugin.GetAssemblyByPluginId(pluginConfig.PluginId), $"{Path.GetFileNameWithoutExtension(pluginConfig.PluginPath)}.{_basePath}") );
        }

        public void PostConfigure(string name, StaticFileOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));
            Dictionary<string, IFileProvider> fileProviders = new Dictionary<string, IFileProvider>();
            var list = PluginUtil.GetAllPlugins().Where(x => x.IsEnable).ToList();
            foreach (var pluginConfig in list)
            {
                DefaultPlugin.LoadPlugin(pluginConfig);
                fileProviders.Add(pluginConfig.PluginId, new EmbeddedFileProvider(DefaultPlugin.GetAssemblyByPluginId(pluginConfig.PluginId), $"{Path.GetFileNameWithoutExtension(pluginConfig.PluginPath)}.{_basePath}"));
            }

            _filesProvider = new MyCompositeFileProvider(fileProviders);
            
            // Basic initialization in case the options weren't initialized by any other component
            options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();
            if (options.FileProvider == null && App.WebHostEnvironment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider ??= App.WebHostEnvironment.WebRootFileProvider;
            
            options.FileProvider = new CompositeFileProvider(options.FileProvider, _filesProvider);
        }
    }
}