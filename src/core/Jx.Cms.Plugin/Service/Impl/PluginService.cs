using BootstrapBlazor.Components;
using System.Reflection;
using Jx.Cms.DbContext.Entities.Settings;
using Jx.Cms.Plugin.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Constants = Jx.Cms.Common.Utils.Constants;
using PackageImportHelper = Jx.Cms.Common.Utils.PackageImportHelper;
using PluginConfig = Jx.Cms.Common.Utils.PluginConfig;

namespace Jx.Cms.Plugin.Service.Impl;

public class PluginService : IPluginService
{
    private const long MaxUploadSize = 200L * 1024 * 1024;

    private readonly ILogger<PluginService> _logger;

    public PluginService(ILogger<PluginService> logger)
    {
        _logger = logger;
    }

    public List<PluginConfig> GetAllPlugins()
    {
        return PluginUtil.GetAllPlugins();
    }

    public PluginConfig GetPluginById(int id)
    {
        var pluginEntity = PluginEntity.Find(id);
        return pluginEntity == null ? null : GetPluginByPluginId(pluginEntity.PluginId);
    }

    public PluginConfig GetPluginByPluginId(string pluginId)
    {
        return GetAllPlugins().FirstOrDefault(x => x.PluginId == pluginId);
    }

    public bool ChangePluginStatus(string pluginId)
    {
        return PluginUtil.ChangePluginStatus(pluginId);
    }

    public bool ChangePluginStatus(int id)
    {
        var pluginEntity = PluginEntity.Find(id);
        return pluginEntity != null && ChangePluginStatus(pluginEntity.PluginId);
    }

    public bool DeletePlugin(string pluginId)
    {
        var result = PluginUtil.DeletePlugin(pluginId);
        if (result)
            if (PluginEntity.Select.Where(x => x.PluginId == pluginId).ToDelete().ExecuteAffrows() == 0)
                return false;

        return result;
    }

    public bool DeletePlugin(int id)
    {
        var plugin = PluginEntity.Find(id);
        return plugin != null && DeletePlugin(plugin.PluginId);
    }

    public async Task<(bool IsSuccess, string Message)> UploadPluginAsync(UploadFile file)
    {
        if (file == null) return (false, "未选择文件。");

        var originFileName = file.OriginFileName ?? string.Empty;
        var extension = Path.GetExtension(originFileName);
        if (!string.Equals(extension, ".zip", StringComparison.OrdinalIgnoreCase))
            return (false, "仅支持 zip 压缩包。");

        var tempDir = PackageImportHelper.CreateTempDirectory("plugin-upload");
        var tempZipPath = Path.Combine(tempDir, "plugin.zip");
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

            var validateResult = ValidatePluginPackage(extractDir);
            if (!validateResult.IsSuccess)
                return (false, validateResult.Message);

            var config = validateResult.Config;
            var sourceDir = validateResult.SourceDirectory;
            var sourceDirectoryName = validateResult.DirectoryName;

            Directory.CreateDirectory(Constants.PluginPath);
            var targetDir = Path.Combine(Constants.PluginPath, sourceDirectoryName);

            var existedById = GetAllPlugins().FirstOrDefault(x =>
                string.Equals(x.PluginId, config.PluginId, StringComparison.OrdinalIgnoreCase));
            var existedDirName = Path.GetFileName(Path.GetDirectoryName(existedById?.PluginPath ?? string.Empty) ??
                                                  string.Empty);
            if (existedById != null &&
                !string.Equals(existedDirName, sourceDirectoryName, StringComparison.OrdinalIgnoreCase))
                return (false, $"相同 PluginId 已存在于其他目录：{existedDirName}");

            if (Directory.Exists(targetDir))
            {
                var oldConfigPath = Path.Combine(targetDir, "plugin.json");
                if (File.Exists(oldConfigPath))
                {
                    var oldConfig = TryReadPluginConfig(oldConfigPath);
                    if (!string.IsNullOrWhiteSpace(oldConfig?.PluginId) &&
                        !string.Equals(oldConfig.PluginId, config.PluginId, StringComparison.OrdinalIgnoreCase))
                        return (false, "目标目录已存在其他插件。");
                }
            }

            var needReEnable = existedById?.IsEnable == true;
            if (needReEnable && !ChangePluginStatus(config.PluginId))
                return (false, "更新前禁用当前插件失败。");
            if (needReEnable) await Task.Delay(500);

            if (Directory.Exists(targetDir))
            {
                backupDir = Path.Combine(Path.GetTempPath(), "jxcms", "plugin-backup",
                    $"{sourceDirectoryName}-{Guid.NewGuid():N}");
                Directory.CreateDirectory(Path.GetDirectoryName(backupDir)!);
                try
                {
                    PackageImportHelper.MoveDirectoryWithRetry(targetDir, backupDir, 20, 300);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    if (needReEnable) ChangePluginStatus(config.PluginId);
                    return (false, "插件文件仍被占用，请稍后重试；如持续失败请重启应用后再更新。");
                }
            }

            try
            {
                PackageImportHelper.CopyDirectory(sourceDir, targetDir);
            }
            catch
            {
                if (Directory.Exists(targetDir))
                    CleanupDirectorySafely(targetDir, "插件目标目录回滚");
                if (!string.IsNullOrWhiteSpace(backupDir) && Directory.Exists(backupDir))
                {
                    if (Directory.Exists(targetDir)) CleanupDirectorySafely(targetDir, "插件目标目录回滚");
                    PackageImportHelper.MoveDirectoryWithRetry(backupDir, targetDir);
                }

                if (needReEnable) ChangePluginStatus(config.PluginId);
                throw;
            }

            if (!string.IsNullOrWhiteSpace(backupDir) && Directory.Exists(backupDir))
                CleanupDirectorySafely(backupDir, "插件备份目录清理");

            var pluginEntity = PluginEntity.Select.Where(x => x.PluginId == config.PluginId).First();
            if (pluginEntity == null)
            {
                new PluginEntity
                {
                    PluginId = config.PluginId,
                    IsEnable = false
                }.Save();
            }

            if (needReEnable)
            {
                if (!ChangePluginStatus(config.PluginId))
                    return (false, "插件包上传成功，但重新启用失败。");
                return (true, $"插件 {config.PluginName} 更新成功并已重新启用。");
            }

            return (true, $"插件 {config.PluginName} 上传成功。");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Plugin upload failed: {FileName}", originFileName);
            return (false, $"上传失败：{ex.Message}");
        }
        finally
        {
            CleanupDirectorySafely(tempDir, "插件临时目录清理");
            if (!string.IsNullOrWhiteSpace(backupDir) && Directory.Exists(backupDir))
                CleanupDirectorySafely(backupDir, "插件备份目录清理");
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

    private static (bool IsSuccess, string Message, PluginConfig Config, string SourceDirectory, string DirectoryName)
        ValidatePluginPackage(string extractDir)
    {
        var configPaths = Directory.GetFiles(extractDir, "plugin.json", SearchOption.AllDirectories);
        if (configPaths.Length == 0)
            return (false, "未找到 plugin.json。", null, null, null);
        if (configPaths.Length > 1)
            return (false, "发现多个 plugin.json，请只保留一个。", null, null, null);

        var configPath = configPaths[0];
        var config = TryReadPluginConfig(configPath);
        if (config == null)
            return (false, "plugin.json 格式无效。", null, null, null);
        if (string.IsNullOrWhiteSpace(config.PluginId))
            return (false, "plugin.json 中缺少 PluginId。", null, null, null);
        if (string.IsNullOrWhiteSpace(config.PluginName))
            return (false, "plugin.json 中缺少 PluginName。", null, null, null);

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

        return (true, string.Empty, config, sourceDir, dirName);
    }

    private static PluginConfig TryReadPluginConfig(string configPath)
    {
        try
        {
            var json = File.ReadAllText(configPath);
            return JsonConvert.DeserializeObject<PluginConfig>(json);
        }
        catch
        {
            return null;
        }
    }
}


