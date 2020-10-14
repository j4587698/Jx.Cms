using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Primitives;

namespace Jx.Cms.Themes.FileProvider
{
    public class MyCompositeFileProvider: IFileProvider
    {
        private List<IFileProvider> _fileProviders;

        public MyCompositeFileProvider(params IFileProvider[] fileProviders) => _fileProviders = fileProviders.ToList();

        public MyCompositeFileProvider(IEnumerable<IFileProvider> fileProviders) => _fileProviders = fileProviders != null ? fileProviders.ToList() : throw new ArgumentNullException(nameof (fileProviders));

        public IFileInfo GetFileInfo(string subpath)
        {
            foreach (IFileProvider fileProvider in _fileProviders)
            {
                IFileInfo fileInfo = fileProvider.GetFileInfo(subpath);
                if (fileInfo != null && fileInfo.Exists)
                    return fileInfo;
            }
            return new NotFoundFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath) => new CompositeDirectoryContents(_fileProviders, subpath);

        public IChangeToken Watch(string pattern)
        {
            List<IChangeToken> changeTokenList = new List<IChangeToken>();
            foreach (IFileProvider fileProvider in this._fileProviders)
            {
                IChangeToken changeToken = fileProvider.Watch(pattern);
                if (changeToken != null)
                    changeTokenList.Add(changeToken);
            }
            return changeTokenList.Count == 0 ? (IChangeToken) NullChangeToken.Singleton : new CompositeChangeToken(changeTokenList);
        }

        public void ModifyFileProvider(params IFileProvider[] fileProviders) => _fileProviders = fileProviders.ToList();

        public IEnumerable<IFileProvider> FileProviders => _fileProviders;
    }
}