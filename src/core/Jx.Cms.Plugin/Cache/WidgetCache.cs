using Jx.Cms.Common.Enum;
using Jx.Cms.Common.Utils;
using Jx.Cms.Common.Vo;
using Jx.Cms.DbContext.Entities.Settings;
using Jx.Cms.Plugin.Plugin;
using Jx.Toolbox.Extensions;
using Newtonsoft.Json;

namespace Jx.Cms.Plugin.Cache;

public class WidgetCache
{
    private static Dictionary<WidgetSidebarType, List<IWidget>> EnabledWidget { get; } = new();

    public static void UpdateCache()
    {
        var widgetSidebarNames = Enum.GetNames<WidgetSidebarType>();
        var widgetSidebarNameSet = new HashSet<string>(widgetSidebarNames, StringComparer.OrdinalIgnoreCase);

        var widgetsVos = SettingsEntity
            .Where(x => x.Type == Constants.SystemType)
            .ToList()
            .Where(x => widgetSidebarNameSet.Contains(x.Name))
            .ToDictionary(x => x.Name,
                x => x.Value.IsNullOrEmpty()
                    ? new List<WidgetVo>()
                    : JsonConvert.DeserializeObject<List<WidgetVo>>(x.Value) ?? new List<WidgetVo>(),
                StringComparer.OrdinalIgnoreCase);
        var widgetTypeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        foreach (var type in AssemblyCache.TypeList.Where(x => !x.IsAbstract && typeof(IWidget).IsAssignableFrom(x)))
        {
            // 优先使用 IWidget.Name（后台持久化字段），并兼容历史上按类型名保存的值。
            if (Activator.CreateInstance(type) is IWidget widget && !widget.Name.IsNullOrEmpty())
            {
                widgetTypeMap.TryAdd(widget.Name, type);
            }

            widgetTypeMap.TryAdd(type.Name, type);
        }

        EnabledWidget.Clear();
        foreach (var name in widgetSidebarNames)
        {
            if (!widgetsVos.TryGetValue(name, out var savedWidgets) ||
                !Enum.TryParse(name, true, out WidgetSidebarType widgetSidebarType)) continue;

            var widgets = new List<IWidget>();
            foreach (var vo in savedWidgets)
            {
                if (vo.Name.IsNullOrEmpty() || !widgetTypeMap.TryGetValue(vo.Name, out var widgetType)) continue;
                if (Activator.CreateInstance(widgetType) is not IWidget widget) continue;
                widget.Parameter = vo.Parameter;
                widgets.Add(widget);
            }

            EnabledWidget.Add(widgetSidebarType, widgets);
        }
    }

    public static List<IWidget> GetSidebarWidgets(WidgetSidebarType widgetSidebarType)
    {
        return EnabledWidget.ContainsKey(widgetSidebarType) ? EnabledWidget[widgetSidebarType] : new List<IWidget>();
    }
}
