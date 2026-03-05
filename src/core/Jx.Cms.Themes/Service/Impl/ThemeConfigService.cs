using Jx.Cms.Common.Utils;
using Jx.Cms.Themes.Util;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Jx.Cms.Themes.Service.Impl;

public class ThemeConfigService : IThemeConfigService
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
        var screenShotPath = GetAllThemes().Where(x => x.ThemeName == themeName)
            .Select(x => Path.Combine(x.Path, x.ScreenShot))
            .FirstOrDefault() ?? "";
        using Image img = LoadImage(screenShotPath);
        var stream = new MemoryStream();
        img.Mutate(x => x.Resize(150, 200));
        img.Save(stream, JpegFormat.Instance);
        stream.Position = 0;
        return stream;
    }

    public bool EnableTheme(ThemeConfig themeConfig)
    {
        if (themeConfig == null)
        {
            _logger.LogWarning("主题配置为空，无法启用");
            return false;
        }

        try
        {
            ThemeUtil.SetTheme(themeConfig);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启用主题失败: {ThemeName}", themeConfig.ThemeName);
            return false;
        }
    }

    private Image LoadImage(string screenShotPath)
    {
        if (File.Exists(screenShotPath))
            try
            {
                return Image.Load(screenShotPath);
            }
            catch
            {
                _logger.LogInformation("获取封面信息失败:{screenShotPath}", screenShotPath);
                return Image.Load(Resource.GetResource("noconver.jpg"));
            }
        return Image.Load(Resource.GetResource("noconver.jpg"));
    }
}
