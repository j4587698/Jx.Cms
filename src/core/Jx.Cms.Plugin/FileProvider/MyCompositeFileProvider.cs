using Jx.Cms.Common.Utils;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Primitives;

namespace Jx.Cms.Plugin.FileProvider;

public class MyCompositeFileProvider : IFileProvider
{
    private readonly Dictionary<string, IFileProvider> _fileProviders;

    public MyCompositeFileProvider(Dictionary<string, IFileProvider> fileProviders)
    {
        _fileProviders = fileProviders;
    }

    public MyCompositeFileProvider()
    {
        _fileProviders = new Dictionary<string, IFileProvider>();
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        if (_fileProviders == null) return new NotFoundDirectoryContents();

        // 过滤掉 null 的文件提供程序
        var validProviders = _fileProviders.Values.Where(p => p != null).ToList();
        if (validProviders.Count == 0) return new NotFoundDirectoryContents();

        return new CompositeDirectoryContents(validProviders, subpath);
    }

    public IFileInfo GetFileInfo(string subPath)
    {
        if (_fileProviders == null) return new NotFoundFileInfo(subPath);

        foreach (var fileProvider in _fileProviders.Values)
        {
            if (fileProvider == null) continue;

            var fileInfo = fileProvider.GetFileInfo(subPath);
            if (fileInfo != null && fileInfo.Exists)
                return fileInfo;
        }

        return new NotFoundFileInfo(subPath);
    }

    public IChangeToken Watch(string filter)
    {
        var changeTokenList = new List<IChangeToken>();
        if (_fileProviders != null)
            foreach (var fileProvider in _fileProviders.Values)
            {
                if (fileProvider == null) continue;

                var changeToken = fileProvider.Watch(filter);
                if (changeToken != null)
                    changeTokenList.Add(changeToken);
            }

        return changeTokenList.Count == 0
            ? new CancellationChangeToken(CancellationToken.None)
            : new CompositeChangeToken(changeTokenList);
    }

    public void ModifyPlugin(PluginConfig pluginConfig, IFileProvider fileProvider)
    {
        if (pluginConfig.IsEnable)
        {
            if (!_fileProviders.ContainsKey(pluginConfig.PluginId))
                _fileProviders.Add(pluginConfig.PluginId, fileProvider);
        }
        else
        {
            if (_fileProviders.ContainsKey(pluginConfig.PluginId)) _fileProviders.Remove(pluginConfig.PluginId);
        }
    }
}