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
    /// <summary>
    ///     主提供器
    /// </summary>
    private List<IFileProvider> _fileProviders;

    /// <summary>
    ///     手机主题提供器
    /// </summary>
    private IFileProvider _mobileFileProvider;

    /// <summary>
    ///     PC主题提供器
    /// </summary>
    private IFileProvider _pcProvider;

    public MyCompositeFileProvider(params IFileProvider[] fileProviders)
    {
        _fileProviders = fileProviders.ToList();
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
        if (_fileProviders == null) return new NotFoundFileInfo(subPath);

        foreach (var fileProvider in _fileProviders)
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
        if (_fileProviders == null) return new NotFoundDirectoryContents();

        // 过滤掉 null 的文件提供程序
        var validProviders = _fileProviders.Where(p => p != null).ToList();
        if (validProviders.Count == 0) return new NotFoundDirectoryContents();

        return new CompositeDirectoryContents(validProviders, subPath);
    }

    public IChangeToken Watch(string pattern)
    {
        var changeTokenList = new List<IChangeToken>();
        if (_fileProviders != null)
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

    private void CreateFileProviders()
    {
        if (_fileProviders == null) _fileProviders = new List<IFileProvider>();
        _fileProviders.Clear();
        if (_pcProvider != null) _fileProviders.Add(_pcProvider);

        if (_mobileFileProvider != null) _fileProviders.Add(_mobileFileProvider);
    }
}