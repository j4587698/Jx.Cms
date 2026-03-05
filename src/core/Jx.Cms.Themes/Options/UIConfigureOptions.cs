using Jx.Cms.Common.Provider;
using Jx.Cms.Common.Utils;
using Jx.Cms.Plugin;
using Jx.Cms.Themes.FileProvider;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Jx.Cms.Themes.Options;

public class UiConfigureOptions : IPostConfigureOptions<StaticFileOptions>
{
    private const string BasePath = "wwwroot";
    private readonly MyCompositeFileProvider _filesProvider = new();
    private readonly IWebHostEnvironment _environment;

    public UiConfigureOptions(IWebHostEnvironment environment)
    {
        _environment = environment;
        ThemeUtil.ThemeModify = ChangeTheme;
        ThemeUtil.InitThemePath();
    }

    public void PostConfigure(string name, StaticFileOptions options)
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        options = options ?? throw new ArgumentNullException(nameof(options));
        options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();

        var providers = new List<IFileProvider>();
        if (_environment.WebRootFileProvider != null) providers.Add(_environment.WebRootFileProvider);
        if (options.FileProvider != null) providers.Add(options.FileProvider);
        providers.Add(_filesProvider);

        options.FileProvider = new SafeCompositeFileProvider(providers);
        Common.Utils.Util.ThemeProvider = options.FileProvider;
    }

    private void ChangeTheme(ThemeConfig themeConfig)
    {
        if (themeConfig == null) return;

        IFileProvider provider = null;
        var assembly = RazorPlugin.GetAssemblyByThemeType(themeConfig.ThemeType);
        if (assembly != null && !string.IsNullOrWhiteSpace(themeConfig.Path))
            provider =
                new EmbeddedFileProvider(assembly, $"{Path.GetFileNameWithoutExtension(themeConfig.Path)}.{BasePath}");

        _filesProvider.ModifyFileProvider(provider, themeConfig.ThemeType);
    }
}
