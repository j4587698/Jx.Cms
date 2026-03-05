using Jx.Cms.Common.Utils;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Primitives;

namespace Jx.Cms.Themes.FileProvider;

/// <summary>
///     用于切换主题
/// </summary>
public class MyCompositeFileProvider : IFileProvider
{
    private readonly object _syncRoot = new();
    private IFileProvider _mobileFileProvider;
    private IFileProvider _pcProvider;
    private IReadOnlyList<IFileProvider> _fileProviders = Array.Empty<IFileProvider>();

    public MyCompositeFileProvider(params IFileProvider[] fileProviders)
    {
        _fileProviders = (fileProviders ?? Array.Empty<IFileProvider>()).Where(x => x != null).ToList();
    }

    public MyCompositeFileProvider(IFileProvider pcProvider, IFileProvider mobileFileProvider)
    {
        _pcProvider = pcProvider;
        _mobileFileProvider = mobileFileProvider;
        CreateFileProviders();
    }

    public IEnumerable<IFileProvider> FileProviders => _fileProviders;

    public IFileInfo GetFileInfo(string subPath)
    {
        var providers = _fileProviders;
        foreach (var fileProvider in providers)
        {
            if (fileProvider == null) continue;

            var fileInfo = fileProvider.GetFileInfo(subPath);
            if (fileInfo is { Exists: true })
                return fileInfo;
        }

        return new NotFoundFileInfo(subPath);
    }

    public IDirectoryContents GetDirectoryContents(string subPath)
    {
        var providers = _fileProviders.Where(p => p != null).ToList();
        return providers.Count == 0 ? new NotFoundDirectoryContents() : new CompositeDirectoryContents(providers, subPath);
    }

    public IChangeToken Watch(string pattern)
    {
        var changeTokenList = new List<IChangeToken>();
        foreach (var fileProvider in _fileProviders)
        {
            if (fileProvider == null) continue;

            var changeToken = fileProvider.Watch(pattern);
            if (changeToken != null)
                changeTokenList.Add(changeToken);
        }

        return changeTokenList.Count == 0 ? NullChangeToken.Singleton : new CompositeChangeToken(changeTokenList);
    }

    public void ModifyFileProvider(IFileProvider fileProvider, ThemeType themeType)
    {
        lock (_syncRoot)
        {
            switch (themeType)
            {
                case ThemeType.PcTheme:
                    _pcProvider = fileProvider;
                    break;
                case ThemeType.MobileTheme:
                    _mobileFileProvider = fileProvider;
                    break;
                case ThemeType.AdaptiveTheme:
                    _pcProvider = fileProvider;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(themeType), themeType, null);
            }

            CreateFileProviders();
        }
    }

    private void CreateFileProviders()
    {
        var providers = new List<IFileProvider>();
        if (_pcProvider != null) providers.Add(_pcProvider);
        if (_mobileFileProvider != null) providers.Add(_mobileFileProvider);
        _fileProviders = providers;
    }
}
