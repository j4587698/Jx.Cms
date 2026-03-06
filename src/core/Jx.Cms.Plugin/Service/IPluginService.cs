using BootstrapBlazor.Components;
using Jx.Cms.Common.Utils;

namespace Jx.Cms.Plugin.Service;

/// <summary>
/// Plugin management service.
/// </summary>
public interface IPluginService
{
    /// <summary>
    /// Get all plugins.
    /// </summary>
    List<PluginConfig> GetAllPlugins();

    /// <summary>
    /// Get plugin by database id.
    /// </summary>
    PluginConfig GetPluginById(int id);

    /// <summary>
    /// Get plugin by plugin id.
    /// </summary>
    PluginConfig GetPluginByPluginId(string pluginId);

    /// <summary>
    /// Enable or disable plugin by plugin id.
    /// </summary>
    bool ChangePluginStatus(string pluginId);

    /// <summary>
    /// Enable or disable plugin by database id.
    /// </summary>
    bool ChangePluginStatus(int id);

    /// <summary>
    /// Delete plugin files and record.
    /// </summary>
    bool DeletePlugin(string pluginId);

    /// <summary>
    /// Delete plugin files and record.
    /// </summary>
    bool DeletePlugin(int id);

    /// <summary>
    /// Upload plugin zip package.
    /// </summary>
    Task<(bool IsSuccess, string Message)> UploadPluginAsync(UploadFile file);
}
