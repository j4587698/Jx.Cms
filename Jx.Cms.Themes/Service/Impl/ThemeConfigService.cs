using System.Collections.Generic;
using System.IO;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.Common.Utils;
using Jx.Cms.Themes.Util;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Jx.Cms.Themes.Service.Impl
{
    public class ThemeConfigService: IThemeConfigService, ITransient
    {

        private readonly ILogger<ThemeConfigService> _logger;

        public ThemeConfigService(ILogger<ThemeConfigService> logger)
        {
            _logger = logger;
        }
        
        public List<ThemeConfig> GetAllThemes()
        {
            return ThemeUtil.GetAllThemes();
        }

        public Stream GetScreenShotStreamByThemeName(string themeName)
        {
            var screenShotPath = GetAllThemes().Where(x => x.ThemeName == themeName).Select(x => Path.Combine(x.Path, x.ScreenShot))
                .FirstOrDefault() ?? "";
            Image img;
            if (File.Exists(screenShotPath))
            {
                try
                {
                    img = Image.Load(screenShotPath);
                }
                catch
                {
                    _logger.LogInformation("获取封面信息失败:{screenShotPath}", screenShotPath);
                    img = Image.Load(Resource.GetResource("noconver.jpg"));
                }
            }
            else
            {
                img = Image.Load(Resource.GetResource("noconver.jpg"));
                
            }
            var stream = new MemoryStream();
            img.Mutate(x => x.Resize(150, 200));
            img.Save(stream, JpegFormat.Instance);
            stream.Position = 0;
            return stream;
        }

        public bool EnableTheme(ThemeConfig themeConfig)
        {
            ThemeUtil.SetTheme(themeConfig);
            return true;
        }
    }
}