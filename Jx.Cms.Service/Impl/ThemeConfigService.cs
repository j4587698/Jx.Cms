﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using Furion.DependencyInjection;
using Jx.Cms.Common;
using Jx.Cms.Common.Utils;
using Jx.Cms.Themes.Util;
using Masuit.Tools.Media;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Jx.Cms.Service.Impl
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
            return Utils.GetAllThemes();
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
                    img = Image.FromFile(screenShotPath);
                }
                catch
                {
                    _logger.LogInformation("获取封面信息失败:{screenShotPath}", screenShotPath);
                    img = Image.FromStream(Resource.GetResource("noconver.jpg"));
                }
            }
            else
            {
                img = Image.FromStream(Resource.GetResource("noconver.jpg"));
                
            }
            var bitmap = new Bitmap(img);
            var stream = new MemoryStream();
            bitmap.ResizeImage(150, 200).Save(stream, ImageFormat.Jpeg);
            stream.Position = 0;
            return stream;
        }

        public bool EnableTheme(ThemeConfig themeConfig)
        {
            Utils.SetTheme(themeConfig);
            
            return true;
        }
    }
}