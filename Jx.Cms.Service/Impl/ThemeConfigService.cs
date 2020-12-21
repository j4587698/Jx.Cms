using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using Furion.DependencyInjection;
using Jx.Cms.Common.Utils;
using Jx.Cms.Themes.Util;
using Masuit.Tools.Media;
using Newtonsoft.Json;

namespace Jx.Cms.Service.Impl
{
    public class ThemeConfigService: IThemeConfigService, ITransient
    {
        public List<ThemeConfig> GetAllThemes()
        {
            var themeConfigs = new List<ThemeConfig>();
            themeConfigs.Add(new ThemeConfig()
            {
                IsUsing = Utils.PcThemeName == "Default",
                Path = "",
                ScreenShot = "",
                ThemeDescription = "默认主题",
                ThemeName = "Default",
                ThemeType = ThemeType.PcTheme
            });
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
                        switch (themeConfig.ThemeType)
                        {
                            case ThemeType.PcTheme:
                                themeConfig.IsUsing = Utils.PcThemeName == themeConfig.ThemeName;
                                break;
                            case ThemeType.MobileTheme:
                                themeConfig.IsUsing = Utils.MobileThemeName == themeConfig.ThemeName;
                                break;
                            case ThemeType.AdaptiveTheme:
                                themeConfig.IsUsing = Utils.PcThemeName == themeConfig.ThemeName;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
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
                    bitmap.ResizeImage(150, 200).Save(stream, ImageFormat.Jpeg);
                    stream.Position = 0;
                    return stream;
                }
                catch
                {
                    var names = GetType().Assembly.GetManifestResourceNames();
                    return this.GetType().Assembly.GetManifestResourceStream("Jx.Cms.Admin.Resources.noconver.jpg");
                }
            }
            var name = GetType().Assembly.GetManifestResourceNames();
            return GetType().Assembly.GetManifestResourceStream("Jx.Cms.Admin.Resources.noconver.jpg");
        }

        public bool EnableTheme(ThemeConfig themeConfig)
        {
            Utils.SetTheme(themeConfig.ThemeName, themeConfig.ThemeType);
            return true;
        }
    }
}