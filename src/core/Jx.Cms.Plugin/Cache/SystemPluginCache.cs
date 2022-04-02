using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jx.Cms.Plugin.Plugin;

namespace Jx.Cms.Plugin.Cache;

public class SystemPluginCache : IPluginCache
{
    private static IEnumerable<Type> _systemTypes;

    public static void UpdateType()
    {
        _systemTypes = AssemblyCache.TypeList.Where(x => typeof(ISystemPlugin).IsAssignableFrom(x) && !x.IsAbstract);
    }

    public static void RemoveAssembly(Assembly assembly)
    {
        var list = assembly.GetTypes().Where(x => typeof(ISystemPlugin).IsAssignableFrom(x) && !x.IsAbstract)
            .Select(x => Activator.CreateInstance(x) as ISystemPlugin);
        foreach (var systemPlugin in list)
        {
            systemPlugin?.PluginDisable();
        }
    }

    public static IEnumerable<ISystemPlugin> GetSystemPlugins()
    {
        return _systemTypes == null ? Array.Empty<ISystemPlugin>() : _systemTypes.Select(x => Activator.CreateInstance(x) as ISystemPlugin);
    }
}