using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Primitives;

namespace Jx.Cms.Common.Provider;

/// <summary>
///     安全的复合文件提供程序，能够处理 null 文件提供程序
/// </summary>
public class SafeCompositeFileProvider : IFileProvider
{
    private readonly List<IFileProvider> _fileProviders;

    public SafeCompositeFileProvider(params IFileProvider[] fileProviders)
    {
        _fileProviders = fileProviders?.Where(p => p != null).ToList() ?? new List<IFileProvider>();
    }

    public SafeCompositeFileProvider(IEnumerable<IFileProvider> fileProviders)
    {
        _fileProviders = fileProviders?.Where(p => p != null).ToList() ?? new List<IFileProvider>();
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (string.IsNullOrEmpty(subpath)) return new NotFoundFileInfo(subpath);

        foreach (var provider in _fileProviders)
        {
            if (provider == null) continue;

            try
            {
                var fileInfo = provider.GetFileInfo(subpath);
                if (fileInfo != null && fileInfo.Exists) return fileInfo;
            }
            catch (NullReferenceException)
            {
                // 只忽略空引用异常，继续尝试其他提供程序
            }
            catch (Exception ex)
            {
                // 对于其他异常，记录并重新抛出
                Debug.WriteLine($"Error getting file info for {subpath}: {ex.Message}");
                throw;
            }
        }

        return new NotFoundFileInfo(subpath);
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        if (string.IsNullOrEmpty(subpath)) return new NotFoundDirectoryContents();

        if (_fileProviders.Count == 0) return new NotFoundDirectoryContents();

        // 如果只有一个提供程序，直接返回其结果
        if (_fileProviders.Count == 1)
            try
            {
                return _fileProviders[0].GetDirectoryContents(subpath);
            }
            catch (NullReferenceException)
            {
                return new NotFoundDirectoryContents();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting directory contents for {subpath}: {ex.Message}");
                throw;
            }

        // 多个提供程序时，使用 CompositeDirectoryContents
        try
        {
            return new CompositeDirectoryContents(_fileProviders, subpath);
        }
        catch (NullReferenceException)
        {
            // 如果 CompositeDirectoryContents 因为空引用失败，尝试逐个返回
            foreach (var provider in _fileProviders)
                try
                {
                    var contents = provider.GetDirectoryContents(subpath);
                    if (contents != null && contents.Exists) return contents;
                }
                catch (NullReferenceException)
                {
                    // 忽略空引用异常
                }

            return new NotFoundDirectoryContents();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating composite directory contents for {subpath}: {ex.Message}");
            throw;
        }
    }

    public IChangeToken Watch(string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return NullChangeToken.Singleton;

        var changeTokens = new List<IChangeToken>();

        foreach (var provider in _fileProviders)
        {
            if (provider == null) continue;

            try
            {
                var changeToken = provider.Watch(pattern);
                if (changeToken != null) changeTokens.Add(changeToken);
            }
            catch (NullReferenceException)
            {
                // 只忽略空引用异常
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error watching pattern {pattern}: {ex.Message}");
                // 对于 Watch 方法，我们不重新抛出异常，而是继续处理其他提供程序
            }
        }

        return changeTokens.Count == 0 ? NullChangeToken.Singleton : new CompositeChangeToken(changeTokens);
    }
}