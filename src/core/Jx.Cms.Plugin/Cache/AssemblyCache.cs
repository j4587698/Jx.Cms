using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Furion;
using Furion.DependencyInjection;

namespace Jx.Cms.Plugin.Cache;

/// <summary>
/// 程序集缓存
/// </summary>
public class AssemblyCache
{
    /// <summary>
    /// 程序集列表
    /// </summary>
    private static readonly List<Assembly> _assemblyList = App.Assemblies.ToList();

    /// <summary>
    /// 类型列表
    /// </summary>
    public static IEnumerable<Type> TypeList { get; private set; } = App.EffectiveTypes;

    /// <summary>
    /// 添加程序集
    /// </summary>
    /// <param name="assembly"></param>
    public static void AddAssembly(Assembly assembly)
    {
        if (_assemblyList.Any(x => x.FullName == assembly.FullName))
        {
            return;
        }
        _assemblyList.Add(assembly);
        TypeList = _assemblyList.SelectMany(u => u.GetTypes()
            .Where(u => u.IsPublic && !u.IsDefined(typeof(SuppressSnifferAttribute), false)));
        var caches = _assemblyList.SelectMany(x =>
            x.GetTypes().Where(y => typeof(IPluginCache).IsAssignableFrom(y) && !y.IsAbstract));
        foreach (var cache in caches)
        {
            cache.InvokeMember(nameof(IPluginCache.UpdateType),
                BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, null, null, Array.Empty<object>() );
        }
    }

    /// <summary>
    /// 移除程序集
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static Assembly RemoveAssembly(Assembly assembly)
    {
        var ass = _assemblyList.Find(x => x.FullName == assembly.FullName);
        if (ass != null)
        {
            _assemblyList.Remove(ass);
            TypeList = _assemblyList.SelectMany(u => u.GetTypes()
                .Where(x => x.IsPublic && !x.IsDefined(typeof(SuppressSnifferAttribute), false)));
            var caches = _assemblyList.SelectMany(x =>
                x.GetTypes().Where(y => typeof(IPluginCache).IsAssignableFrom(y) && !y.IsAbstract));
            foreach (var cache in caches)
            {
                cache.InvokeMember(nameof(IPluginCache.RemoveAssembly),
                    BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, null, null, new object[]{ass} );
                cache.InvokeMember(nameof(IPluginCache.UpdateType),
                    BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, null, null, Array.Empty<object>() );
            }
        }

        return ass;
    }
}