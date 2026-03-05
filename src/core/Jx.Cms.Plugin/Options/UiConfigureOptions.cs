using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin.FileProvider;
using Jx.Cms.Plugin.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Jx.Cms.Plugin.Options;

public class UiConfigureOptions : IPostConfigureOptions<StaticFileOptions>
{
    private readonly string _basePath = "wwwroot";

    private MyCompositeFileProvider _filesProvider;

    public UiConfigureOptions()
    {
        PluginUtil.PluginModify = ModifyPlugin;
    }

    public void PostConfigure(string name, StaticFileOptions options)
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        options = options ?? throw new ArgumentNullException(nameof(options));
        var fileProviders = new Dictionary<string, IFileProvider>();
        if (Util.IsInstalled)
        {
            var list = PluginUtil.GetAllPlugins().Where(x => x.IsEnable).ToList();
            foreach (var pluginConfig in list)
            {
                DefaultPlugin.LoadPlugin(pluginConfig);
                fileProviders.Add(pluginConfig.PluginId,
                    new EmbeddedFileProvider(DefaultPlugin.GetAssemblyByPluginId(pluginConfig.PluginId),
                        $"{Path.GetFileNameWithoutExtension(pluginConfig.PluginPath)}.{_basePath}"));
            }
        }

        _filesProvider = new MyCompositeFileProvider(fileProviders);

        // Basic initialization in case the options weren't initialized by any other component
        options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();

        options.FileProvider = options.FileProvider == null
            ? _filesProvider
            : new CompositeFileProvider(options.FileProvider, _filesProvider);
    }

    public void ModifyPlugin(PluginConfig pluginConfig)
    {
        if (pluginConfig == null || string.IsNullOrWhiteSpace(pluginConfig.PluginPath) || _filesProvider == null) return;

        // 禁用插件时只需要从组合文件提供器中移除，避免依赖卸载后的程序集实例。
        if (!pluginConfig.IsEnable)
        {
            _filesProvider.ModifyPlugin(pluginConfig, null);
            return;
        }

        var assembly = DefaultPlugin.GetAssemblyByPluginId(pluginConfig.PluginId);
        if (assembly == null) return;

        _filesProvider.ModifyPlugin(pluginConfig,
            new EmbeddedFileProvider(assembly,
                $"{Path.GetFileNameWithoutExtension(pluginConfig.PluginPath)}.{_basePath}"));
    }
}
