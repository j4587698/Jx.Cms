using BootstrapBlazor.Components;
using System.Reflection;
using Jx.Cms.Common.Utils;
using Jx.Cms.Themes.Util;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Constants = Jx.Cms.Common.Utils.Constants;
using PackageImportHelper = Jx.Cms.Common.Utils.PackageImportHelper;

namespace Jx.Cms.Themes.Service.Impl;

public class ThemeConfigService : IThemeConfigService
{
    private const long MaxUploadSize = 300L * 1024 * 1024;

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
            .FirstOrDefault() ?? string.Empty;
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
            _logger.LogWarning("Theme config is null.");
            return false;
        }

        try
        {
            ThemeUtil.SetTheme(themeConfig);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Enable theme failed: {ThemeName}", themeConfig.ThemeName);
            return false;
        }
    }

    public async Task<(bool IsSuccess, string Message)> UploadThemeAsync(UploadFile file)
    {
        if (file == null) return (false, "未选择文件。");

        var originFileName = file.OriginFileName ?? string.Empty;
        var extension = Path.GetExtension(originFileName);
        if (!string.Equals(extension, ".zip", StringComparison.OrdinalIgnoreCase))
            return (false, "仅支持 zip 压缩包。");

        var tempDir = PackageImportHelper.CreateTempDirectory("theme-upload");
        var tempZipPath = Path.Combine(tempDir, "theme.zip");
        var extractDir = Path.Combine(tempDir, "extract");
        var backupDir = string.Empty;

        try
        {
            if (!await file.SaveToFileAsync(tempZipPath, MaxUploadSize))
            {
                var error = string.IsNullOrWhiteSpace(file.Error) ? "保存失败" : file.Error;
                return (false, $"上传失败：{error}");
            }

            PackageImportHelper.ExtractZipSafely(tempZipPath, extractDir);

            var validateResult = ValidateThemePackage(extractDir);
            if (!validateResult.IsSuccess)
                return (false, validateResult.Message);

            var config = validateResult.Config;
            var sourceDir = validateResult.SourceDirectory;
            var sourceDirectoryName = validateResult.DirectoryName;

            Directory.CreateDirectory(Constants.ThemePath);
            var targetDir = Path.Combine(Constants.ThemePath, sourceDirectoryName);

            var existedTheme = GetAllThemes().FirstOrDefault(x =>
                string.Equals(x.ThemeName, config.ThemeName, StringComparison.OrdinalIgnoreCase) &&
                x.ThemeType == config.ThemeType);
            var existedDirName = Path.GetFileName(existedTheme?.Path ?? string.Empty);
            if (existedTheme != null &&
                !string.Equals(existedDirName, sourceDirectoryName, StringComparison.OrdinalIgnoreCase))
                return (false, $"同名主题已存在于其他目录：{existedDirName}");

            var wasUsing = IsThemeUsing(config);
            ThemeConfig restoreConfig = null;
            if (wasUsing)
            {
                restoreConfig = new ThemeConfig
                {
                    ThemeName = config.ThemeName,
                    ThemeDisplayName = config.ThemeDisplayName,
                    ThemeDescription = config.ThemeDescription,
                    ThemeType = config.ThemeType,
                    Path = targetDir,
                    ScreenShot = string.IsNullOrWhiteSpace(config.ScreenShot) ? "screenshot.jpg" : config.ScreenShot
                };

                var fallbackConfig = CreateFallbackThemeConfig(config.ThemeType);
                if (!EnableTheme(fallbackConfig))
                    return (false, "更新前切换到默认主题失败。");
                await Task.Delay(500);
            }

            if (Directory.Exists(targetDir))
            {
                backupDir = Path.Combine(Path.GetTempPath(), "jxcms", "theme-backup",
                    $"{sourceDirectoryName}-{Guid.NewGuid():N}");
                Directory.CreateDirectory(Path.GetDirectoryName(backupDir)!);
                try
                {
                    PackageImportHelper.MoveDirectoryWithRetry(targetDir, backupDir, 20, 300);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    if (wasUsing && restoreConfig != null) EnableTheme(restoreConfig);
                    return (false, "主题文件仍被占用，请稍后重试；如持续失败请重启应用后再更新。");
                }
            }

            try
            {
                PackageImportHelper.CopyDirectory(sourceDir, targetDir);
            }
            catch
            {
                if (Directory.Exists(targetDir))
                    CleanupDirectorySafely(targetDir, "主题目标目录回滚");
                if (!string.IsNullOrWhiteSpace(backupDir) && Directory.Exists(backupDir))
                {
                    if (Directory.Exists(targetDir)) CleanupDirectorySafely(targetDir, "主题目标目录回滚");
                    PackageImportHelper.MoveDirectoryWithRetry(backupDir, targetDir);
                }

                if (wasUsing && restoreConfig != null) EnableTheme(restoreConfig);
                throw;
            }

            if (!string.IsNullOrWhiteSpace(backupDir) && Directory.Exists(backupDir))
                CleanupDirectorySafely(backupDir, "主题备份目录清理");

            if (wasUsing && restoreConfig != null)
            {
                if (!EnableTheme(restoreConfig))
                    return (false, "主题包上传成功，但恢复当前主题失败。");
                return (true, $"主题 {config.ThemeDisplayName} 更新成功并已恢复启用。");
            }

            return (true, $"主题 {config.ThemeDisplayName} 上传成功。");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Theme upload failed: {FileName}", originFileName);
            return (false, $"上传失败：{ex.Message}");
        }
        finally
        {
            CleanupDirectorySafely(tempDir, "主题临时目录清理");
            if (!string.IsNullOrWhiteSpace(backupDir) && Directory.Exists(backupDir))
                CleanupDirectorySafely(backupDir, "主题备份目录清理");
        }
    }

    private void CleanupDirectorySafely(string directory, string actionName)
    {
        try
        {
            if (PackageImportHelper.TryDeleteDirectoryWithRetry(directory, 12, 300)) return;
            _logger.LogWarning("{ActionName}失败，目录仍存在：{Directory}", actionName, directory);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{ActionName}异常：{Directory}", actionName, directory);
        }
    }

    private static (bool IsSuccess, string Message, ThemeConfig Config, string SourceDirectory, string DirectoryName)
        ValidateThemePackage(string extractDir)
    {
        var configPaths = Directory.GetFiles(extractDir, "theme.json", SearchOption.AllDirectories);
        if (configPaths.Length == 0)
            return (false, "未找到 theme.json。", null, null, null);
        if (configPaths.Length > 1)
            return (false, "发现多个 theme.json，请只保留一个。", null, null, null);

        var configPath = configPaths[0];
        var config = TryReadThemeConfig(configPath);
        if (config == null)
            return (false, "theme.json 格式无效。", null, null, null);
        if (string.IsNullOrWhiteSpace(config.ThemeName))
            return (false, "theme.json 中缺少 ThemeName。", null, null, null);
        if (string.IsNullOrWhiteSpace(config.ThemeDisplayName))
            return (false, "theme.json 中缺少 ThemeDisplayName。", null, null, null);
        if (!Enum.IsDefined(typeof(ThemeType), config.ThemeType))
            return (false, "theme.json 中 ThemeType 无效。", null, null, null);

        var sourceDir = Path.GetDirectoryName(configPath)!;
        var dirName = Path.GetFileName(sourceDir);
        var dllPath = Path.Combine(sourceDir, $"{dirName}.dll");
        if (!File.Exists(dllPath))
            return (false, $"未找到主程序集 {dirName}.dll。", null, null, null);

        try
        {
            _ = AssemblyName.GetAssemblyName(dllPath);
        }
        catch (Exception ex)
        {
            return (false, $"程序集校验失败：{ex.Message}", null, null, null);
        }

        if (string.IsNullOrWhiteSpace(config.ScreenShot))
            config.ScreenShot = "screenshot.jpg";

        return (true, string.Empty, config, sourceDir, dirName);
    }

    private static ThemeConfig TryReadThemeConfig(string configPath)
    {
        try
        {
            var json = File.ReadAllText(configPath);
            return JsonConvert.DeserializeObject<ThemeConfig>(json);
        }
        catch
        {
            return null;
        }
    }

    private static bool IsThemeUsing(ThemeConfig themeConfig)
    {
        return themeConfig.ThemeType switch
        {
            ThemeType.PcTheme => string.Equals(ThemeUtil.PcThemeName, themeConfig.ThemeName,
                StringComparison.OrdinalIgnoreCase),
            ThemeType.AdaptiveTheme => string.Equals(ThemeUtil.PcThemeName, themeConfig.ThemeName,
                StringComparison.OrdinalIgnoreCase),
            ThemeType.MobileTheme => string.Equals(ThemeUtil.MobileThemeName, themeConfig.ThemeName,
                StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    private static ThemeConfig CreateFallbackThemeConfig(ThemeType themeType)
    {
        return new ThemeConfig
        {
            ThemeName = "Default",
            ThemeDisplayName = "默认主题",
            ThemeDescription = "默认主题",
            ThemeType = themeType,
            Path = string.Empty,
            ScreenShot = string.Empty
        };
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
                _logger.LogInformation("Load screenshot failed: {screenShotPath}", screenShotPath);
                return Image.Load(Resource.GetResource("noconver.jpg"));
            }
        return Image.Load(Resource.GetResource("noconver.jpg"));
    }
}


