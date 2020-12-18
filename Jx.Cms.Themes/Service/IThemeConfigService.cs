using System.Collections.Generic;
using System.IO;
using Jx.Cms.Themes.Util;

namespace Jx.Cms.Themes.Service
{
    public interface IThemeConfigService
    {
        List<ThemeConfig> GetAllThemes();

        Stream GetScreenShotStreamByThemeName(string themeName);
    }
}