using Jx.Cms.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jx.Cms.Plugin.Utils;

internal static class PluginInstanceFactory
{
    private static void LogCreateFailure(Type type, Exception ex)
    {
        var typeName = type?.FullName ?? "UnknownType";
        var message = $"插件实例创建失败：{typeName}，错误：{ex.Message}";
        try
        {
            ServicesExtension.GetService<ILoggerFactory>()
                ?.CreateLogger("Jx.Cms.Plugin.Hook")
                .LogWarning(ex, "{Message}", message);
        }
        catch
        {
            // 日志异常不影响主流程。
        }
    }

    public static T CreateInstance<T>(Type type) where T : class
    {
        if (type == null) return null;

        var serviceProvider = ServicesExtension.ServiceProvider;
        if (serviceProvider != null)
        {
            try
            {
                return ActivatorUtilities.CreateInstance(serviceProvider, type) as T;
            }
            catch (Exception ex)
            {
                LogCreateFailure(type, ex);
            }
        }

        try
        {
            return Activator.CreateInstance(type) as T;
        }
        catch (Exception ex)
        {
            LogCreateFailure(type, ex);
            return null;
        }
    }
}
