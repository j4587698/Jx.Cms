using System.Collections.Generic;
using System.Linq;
using Jx.Cms.Common.Utils;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Primitives;
using StackExchange.Profiling.Internal;

namespace Jx.Cms.Plugin.FileProvider
{
    public class MyCompositeFileProvider: IFileProvider
    {

        private Dictionary<string, IFileProvider> _fileProviders;

        public MyCompositeFileProvider(Dictionary<string, IFileProvider> fileProviders)
        {
            _fileProviders = fileProviders;
        }

        public MyCompositeFileProvider()
        {
            _fileProviders = new Dictionary<string, IFileProvider>();
        }

        public void ModifyPlugin(PluginConfig pluginConfig, IFileProvider fileProvider)
        {
            if (pluginConfig.IsEnable)
            {
                if (!_fileProviders.ContainsKey(pluginConfig.PluginId))
                {
                    _fileProviders.Add(pluginConfig.PluginId, fileProvider);
                }
            }
            else
            {
                _fileProviders.TryRemove(pluginConfig.PluginId, out _);
            }
        }
        
        public IDirectoryContents GetDirectoryContents(string subpath) => new CompositeDirectoryContents(_fileProviders.Values.ToList(), subpath);

        public IFileInfo GetFileInfo(string subPath)
        {
            foreach (IFileProvider fileProvider in _fileProviders.Values)
            {
                IFileInfo fileInfo = fileProvider.GetFileInfo(subPath);
                if (fileInfo != null && fileInfo.Exists)
                    return fileInfo;
            }
            return new NotFoundFileInfo(subPath);
        }

        public IChangeToken Watch(string filter)
        {
            List<IChangeToken> changeTokenList = new List<IChangeToken>();
            foreach (IFileProvider fileProvider in this._fileProviders.Values)
            {
                IChangeToken changeToken = fileProvider.Watch(filter);
                if (changeToken != null)
                    changeTokenList.Add(changeToken);
            }
            return changeTokenList.Count == 0 ? (IChangeToken) NullChangeToken.Singleton : new CompositeChangeToken(changeTokenList);
        }
    }
}