using System.Reflection;
using Jx.Cms.Plugin.Plugin;
using Jx.Cms.Plugin.Utils;

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
        return _articleTypes == null
            ? Array.Empty<IArticlePlugin>()
            : _articleTypes.Select(PluginInstanceFactory.CreateInstance<IArticlePlugin>).Where(x => x != null);
    }
}
