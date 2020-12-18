using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Themes.Util;
using Newtonsoft.Json;

namespace Jx.Cms.Themes.Service.Impl
{
    public class ThemeConfigService: IThemeConfigService, ITransient
    {
        public List<ThemeConfig> GetAllThemes()
        {
            var themeConfigs = new List<ThemeConfig>();
            var dirs = Directory.GetDirectories(Constants.LibraryPath);
            foreach (var dir in dirs)
            {
                var configPath = Path.Combine(dir, "theme.json");
                if (File.Exists(configPath))
                {
                    try
                    {
                        var themeConfig = JsonConvert.DeserializeObject<ThemeConfig>(File.ReadAllText(configPath));
                        themeConfig.Path = dir;
                        themeConfigs.Add(themeConfig);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        //throw;
                    }
                }
            }
            return themeConfigs;
        }

        public Stream GetScreenShotStreamByThemeName(string themeName)
        {
            var screenShotPath = GetAllThemes().Where(x => x.ThemeName == themeName).Select(x => Path.Combine(x.Path, x.ScreenShot))
                .FirstOrDefault() ?? "";
            if (File.Exists(screenShotPath))
            {
                try
                {
                    Image img = Image.FromFile(screenShotPath);
                    var bitmap = new Bitmap(img);
                    var stream = new MemoryStream();
                    var bit = bitmap.ResizeImage(100, 200);
                    bit.Save(stream, ImageFormat.Jpeg);
                    return stream;
                }
                catch
                {
                    return new FileStream("", FileMode.Open, FileAccess.Read);
                }
            }
            return new FileStream("", FileMode.Open, FileAccess.Read);
        }
    }
}