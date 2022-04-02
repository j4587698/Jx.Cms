using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jx.Cms.Plugin.Plugin;

namespace Jx.Cms.Plugin.Cache;

public class ArticlePluginCache : IPluginCache
{
    private static IEnumerable<Type> _articleTypes;

    public static void UpdateType()
    {
        _articleTypes = AssemblyCache.TypeList.Where(x => typeof(IArticlePlugin).IsAssignableFrom(x) && !x.IsAbstract);
    }

    public static void RemoveAssembly(Assembly assembly)
    {
        
    }

    public static IEnumerable<IArticlePlugin> GetArticlePlugins()
    {
        return _articleTypes == null ? Array.Empty<IArticlePlugin>() :_articleTypes.Select(x => Activator.CreateInstance(x) as IArticlePlugin);
    }
}