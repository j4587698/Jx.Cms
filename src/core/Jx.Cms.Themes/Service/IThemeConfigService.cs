using BootstrapBlazor.Components;
using Jx.Cms.Common.Utils;

namespace Jx.Cms.Themes.Service;

/// <summary>
/// Theme management service.
/// </summary>
public interface IThemeConfigService
{
    /// <summary>
    /// Get all themes.
    /// </summary>
    List<ThemeConfig> GetAllThemes();

    /// <summary>
    /// Get screenshot stream by theme name.
    /// </summary>
    Stream GetScreenShotStreamByThemeName(string themeName);

    /// <summary>
    /// Enable a theme.
    /// </summary>
    bool EnableTheme(ThemeConfig themeConfig);

    /// <summary>
    /// Upload theme zip package, overwrite if same theme exists.
    /// </summary>
    Task<(bool IsSuccess, string Message)> UploadThemeAsync(UploadFile file);
}
