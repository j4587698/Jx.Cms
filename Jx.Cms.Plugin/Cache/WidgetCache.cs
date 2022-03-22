using System;
using System.Collections.Generic;
using System.Linq;
using Furion;
using Furion.JsonSerialization;
using Jx.Cms.Common.Enum;
using Jx.Cms.Common.Utils;
using Jx.Cms.Common.Vo;
using Jx.Cms.DbContext.Entities.Settings;
using Jx.Cms.Entities.Settings;
using Jx.Cms.Plugin.Plugin;
using Masuit.Tools;

namespace Jx.Cms.Plugin.Cache;

public class WidgetCache
{
    private static Dictionary<WidgetSidebarType, List<IWidget>> EnabledWidget { get; set; } = new();

    public static void UpdateCache()
    {
        var widgetsVos = SettingsEntity
            .Where(x => x.Type == Constants.SystemType &&
                        Enum.GetNames(typeof(WidgetSidebarType)).Contains(x.Name))
            .ToDictionary(x => x.Name, x => x.Value.IsNullOrEmpty() ? new List<WidgetVo>() : JSON.Deserialize<List<WidgetVo>>(x.Value));
        var widgetTypes = AssemblyCache.TypeList.Where(x => !x.IsAbstract && x.GetInterfaces().Contains(typeof(IWidget)))
            .Select(x => Activator.CreateInstance(x) as IWidget).ToList();
        EnabledWidget.Clear();
        foreach (var name in Enum.GetNames(typeof(WidgetSidebarType)))
        {
            if (!widgetsVos.ContainsKey(name) || !Enum.TryParse(name, true, out WidgetSidebarType widgetSidebarType))
            {
                continue;
            }

            var widgets = new List<IWidget>();
            foreach (var vo in widgetsVos[name])
            {
                var type = widgetTypes.FirstOrDefault(x => x.Name == vo.Name);
                if (type == null) continue;
                var widget = Activator.CreateInstance(type.GetType()) as IWidget;
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