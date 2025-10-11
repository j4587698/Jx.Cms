using System.Reflection;

namespace Jx.Cms.Common.Extensions;

public static class ObjectExtension
{
    public static void CopyTo<TSource, TTarget>(this TSource source, TTarget target)
    {
        var propertyInfos = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var targetType = target.GetType();
        foreach (var propertyInfo in propertyInfos)
        {
            var value = propertyInfo.GetValue(source, null);
            if (value != null) targetType.GetProperty(propertyInfo.Name)?.SetValue(target, value, null);
        }
    }

    public static void CopyFrom<TSource, TTarget>(this TSource source, TTarget target)
    {
        CopyTo(target, source);
    }
}