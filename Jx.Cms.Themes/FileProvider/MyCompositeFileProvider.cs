using System;
using System.Collections.Generic;
using System.Linq;
using Jx.Cms.Common.Utils;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Primitives;

namespace Jx.Cms.Themes.FileProvider
{
    /// <summary>
    /// 用于切换主题
    /// </summary>
    public class MyCompositeFileProvider: IFileProvider
    {
        /// <summary>
        /// 主提供器
        /// </summary>
        private List<IFileProvider> _fileProviders;

        /// <summary>
        /// PC主题提供器
        /// </summary>
        private IFileProvider _pcProvider;

        /// <summary>
        /// 手机主题提供器
        /// </summary>
        private IFileProvider _mobileFileProvider;

        public MyCompositeFileProvider(params IFileProvider[] fileProviders) => _fileProviders = fileProviders.ToList();

        public MyCompositeFileProvider(IFileProvider pcProvider, IFileProvider mobileFileProvider)
        {
            _pcProvider = pcProvider;
            _mobileFileProvider = mobileFileProvider;
            CreateFileProviders();
        }

        public IFileInfo GetFileInfo(string subPath)
        {
            foreach (IFileProvider fileProvider in _fileProviders)
            {
                IFileInfo fileInfo = fileProvider.GetFileInfo(subPath);
                if (fileInfo != null && fileInfo.Exists)
                    return fileInfo;
            }
            return new NotFoundFileInfo(subPath);
        }

        public IDirectoryContents GetDirectoryContents(string subPath) => new CompositeDirectoryContents(_fileProviders, subPath);

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
            if (_fileProviders == null)
            {
                _fileProviders = new List<IFileProvider>();
            }
            _fileProviders.Clear();
            if (_pcProvider != null)
            {
                _fileProviders.Add(_pcProvider);
            }

            if (_mobileFileProvider != null)
            {
                _fileProviders.Add(_mobileFileProvider);
            }
        }

        public IEnumerable<IFileProvider> FileProviders => _fileProviders;
    }
}