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
        var widgetTypeMap = AssemblyCache.TypeList
            .Where(x => !x.IsAbstract && typeof(IWidget).IsAssignableFrom(x))
            .ToDictionary(x => x.Name, x => x, StringComparer.Ordinal);

        EnabledWidget.Clear();
        foreach (var name in widgetSidebarNames)
        {
            if (!widgetsVos.TryGetValue(name, out var savedWidgets) ||
                !Enum.TryParse(name, true, out WidgetSidebarType widgetSidebarType)) continue;

            var widgets = new List<IWidget>();
            foreach (var vo in savedWidgets)
            {
                if (!widgetTypeMap.TryGetValue(vo.Name, out var widgetType)) continue;
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
